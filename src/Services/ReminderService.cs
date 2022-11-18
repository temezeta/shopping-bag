using AutoMapper;
using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.DTOs.Reminder;
using shopping_bag.Models;
using shopping_bag.Models.User;
using System.Collections.Generic;
using System.Text;

namespace shopping_bag.Services {
    public class ReminderService : IReminderService {

        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public ReminderService(AppDbContext context, IUserService userService, IMapper mapper, IEmailService emailService) {
            _context = context;
            _userService = userService;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<ServiceResponse<ReminderSettings>> SetGlobalReminderSettings(long userId, ReminderSettingsDto settings) {
            var resp = await _userService.GetUserById(userId);
            if(!resp.IsSuccess) {
                return new ServiceResponse<ReminderSettings>("Invalid user");
            }
            if(!IsValidReminderInterval(settings.ReminderDaysBeforeDueDate)) {
                return new ServiceResponse<ReminderSettings>("Invalid due date reminder interval");
            }
            if (!IsValidReminderInterval(settings.ReminderDaysBeforeExpectedDate)) {
                return new ServiceResponse<ReminderSettings>("Invalid expected delivery date reminder interval");
            }
            var user = resp.Data;
            if (user.ReminderSettings == null) {
                user.ReminderSettings = new ReminderSettings() {
                    UserId = userId,
                    ReminderDaysBeforeDueDate = settings.ReminderDaysBeforeDueDate,
                    ReminderDaysBeforeExpectedDate = settings.ReminderDaysBeforeExpectedDate,
                    DueDateRemindersDisabled = settings.DueDateRemindersDisabled,
                    ExpectedRemindersDisabled = settings.ExpectedRemindersDisabled
                };
            } else {
                user.ReminderSettings.ReminderDaysBeforeDueDate = settings.ReminderDaysBeforeDueDate;
                user.ReminderSettings.ReminderDaysBeforeExpectedDate = settings.ReminderDaysBeforeExpectedDate;
                user.ReminderSettings.DueDateRemindersDisabled = settings.DueDateRemindersDisabled;
                user.ReminderSettings.ExpectedRemindersDisabled = settings.ExpectedRemindersDisabled;
            }
            await _context.SaveChangesAsync();
            return new ServiceResponse<ReminderSettings>(user.ReminderSettings);
        }

        public async Task CreateRemindersForList(long listId, long officeId) {
            var users = await _context.Users.Include(u => u.ReminderSettings).Include(u => u.Reminders).Where(u => u.OfficeId == officeId && !u.Removed).ToListAsync();
            foreach (var user in users) {
                var reminderSettings = user.ReminderSettings;
                if (reminderSettings == null || (reminderSettings.DueDateRemindersDisabled && reminderSettings.ExpectedRemindersDisabled)) {
                    continue;
                }
                if(user.Reminders.Any(r => r.ShoppingListId == listId)) {
                    // Skip if user already has reminders created on this list.
                    continue;
                }
                var reminder = new Reminder() {
                    ShoppingListId = listId,
                    UserId = user.Id,
                    DueDaysBefore = reminderSettings.ReminderDaysBeforeDueDate,
                    ExpectedDaysBefore = reminderSettings.ReminderDaysBeforeExpectedDate
                };
                user.Reminders.Add(reminder);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<ServiceResponse<User>> SetListReminder(long userId, ReminderSettingsDto settings, long listId) {
            var resp = await _userService.GetUserById(userId);
            if(!resp.IsSuccess || resp.Data == null || resp.Data.Removed) {
                return new ServiceResponse<User>("Invalid user");
            }
            var user = resp.Data;
            if (!(await _context.ShoppingLists.AnyAsync(l => l.Id == listId && !l.Removed))) {
                return new ServiceResponse<User>("Invalid list");
            }
            var reminder = user.Reminders.FirstOrDefault(r => r.ShoppingListId == listId);

            // If both reminders are disabled, or both are empty, then remove the reminder and return null.
            if((settings.DueDateRemindersDisabled && settings.ExpectedRemindersDisabled) ||
                (settings.ReminderDaysBeforeDueDate.Count == 0 && settings.ReminderDaysBeforeExpectedDate.Count == 0)) {
                if (reminder != null) {
                    user.Reminders.Remove(reminder);
                    await _context.SaveChangesAsync();
                }
                return new ServiceResponse<User>(user);
            }

            if (!IsValidReminderInterval(settings.ReminderDaysBeforeDueDate)) {
                return new ServiceResponse<User>("Invalid due date reminder interval");
            }
            if (!IsValidReminderInterval(settings.ReminderDaysBeforeExpectedDate)) {
                return new ServiceResponse<User>("Invalid expected delivery date reminder interval");
            }

            if (reminder != null) {
                reminder.DueDaysBefore = settings.DueDateRemindersDisabled ? new List<int>() : settings.ReminderDaysBeforeDueDate;
                reminder.ExpectedDaysBefore = settings.ExpectedRemindersDisabled ? new List<int>() : settings.ReminderDaysBeforeExpectedDate;
            } else {
                reminder = new Reminder() {
                    ShoppingListId = listId,
                    UserId = userId,
                    DueDaysBefore = settings.DueDateRemindersDisabled ? new List<int>() : settings.ReminderDaysBeforeDueDate,
                    ExpectedDaysBefore = settings.ExpectedRemindersDisabled ? new List<int>() : settings.ReminderDaysBeforeExpectedDate
                };
                user.Reminders.Add(reminder);
            }
            await _context.SaveChangesAsync();
            return new ServiceResponse<User>(user);
        }

        private bool IsValidReminderInterval(List<int> interval) {
            if(interval == null) {
                return true;
            }
            if(interval.Any(i => i <= 0)) {
                return false;
            }
            // Only allow different intervals, can't have two same reminders in one day
            if(interval.Count != interval.Distinct().Count()) {
                return false;
            }
            return true;
        }

        public async Task SendReminders(CancellationToken stoppingToken) {
            var reminders = await _context.Reminders.Include(r => r.ShoppingList).Include(r => r.User).ThenInclude(u => u.ReminderSettings).ToListAsync(cancellationToken: stoppingToken);
            var remindersToSend = new Dictionary<User, List<string>>(new UserEqualityComparer()); // Used to gather possible multiple reminders to be sent in one email.
            foreach (var reminder in reminders) {
                var settings = reminder.User.ReminderSettings;
                var list = reminder.ShoppingList;

                // If list/user removed or both settings disabled
                if (settings == null || reminder.ShoppingList.Removed || reminder.User.Removed || (settings.DueDateRemindersDisabled && settings.ExpectedRemindersDisabled)) {
                    reminder.User.Reminders.Remove(reminder);
                    continue;
                }

                HandleDueDateReminder(reminder, remindersToSend);
                HandleExpectedDateReminder(reminder, remindersToSend);

                // Remove empty reminders
                if (!reminder.DueDaysBefore.Any() && !reminder.ExpectedDaysBefore.Any()) {
                    reminder.User.Reminders.Remove(reminder);
                }
            }
            // Send emails
            SendReminderEmails(remindersToSend);

            await _context.SaveChangesAsync(stoppingToken);
        }

        private void HandleDueDateReminder(Reminder reminder, Dictionary<User, List<string>> remindersToSend) {
            var list = reminder.ShoppingList;
            if (reminder.DueDaysBefore.Any()) {
                int daysBefore = reminder.DueDaysBefore.Max();
                if (IsDateWithinReminderInterval(list.DueDate, daysBefore)) {
                    reminder.DueDaysBefore.Remove(daysBefore);
                    var msg = $"Due date for list {list.Name} is {(daysBefore == 1 ? "tomorrow" : $"in {daysBefore} days")} {list.DueDate.Value.ToString("dd.MM.yyyy")}. Items should be added before the due date.";
                    if (remindersToSend.TryGetValue(reminder.User, out List<string>? msgList)) {
                        msgList.Add(msg);
                    } else {
                        remindersToSend.Add(reminder.User, new List<string>() { msg });
                    }
                }
            }
        }
        private void HandleExpectedDateReminder(Reminder reminder, Dictionary<User, List<string>> remindersToSend) {
            var list = reminder.ShoppingList;
            if (reminder.ExpectedDaysBefore.Any()) {
                int daysBefore = reminder.ExpectedDaysBefore.Max();
                if (IsDateWithinReminderInterval(list.ExpectedDeliveryDate, daysBefore)) {
                    reminder.ExpectedDaysBefore.Remove(daysBefore);
                    var msg = $"Expected delivery date for list {list.Name} is {(daysBefore == 1 ? "tomorrow" : $"in {daysBefore} days")} {list.ExpectedDeliveryDate.Value.ToString("dd.MM.yyyy")}.";
                    if (remindersToSend.TryGetValue(reminder.User, out List<string>? msgList)) {
                        msgList.Add(msg);
                    } else {
                        remindersToSend.Add(reminder.User, new List<string>() { msg });
                    }
                }
            }
        }

        private void SendReminderEmails(Dictionary<User, List<string>> remindersToSend) {
            foreach (var kv in remindersToSend) {
                var sb = new StringBuilder();
                sb.AppendLine("TODO: Something about why this email was sent.");
                sb.AppendJoin(Environment.NewLine, kv.Value); // List of reminders
                sb.Append(Environment.NewLine);
                sb.AppendLine("TODO: Something about how to turn off notifications");
                try {
                    _emailService.SendEmail(new Models.Email.Email() {
                        To = kv.Key.Email,
                        Subject = "TODO: subject",
                        Body = sb.ToString()
                    });
                }catch (Exception) {
                    continue;
                }
            }
        }

        private class UserEqualityComparer : IEqualityComparer<User> {
            public int GetHashCode(User user) { return user.Id.GetHashCode(); }
            public bool Equals(User? user1, User? user2) { return user1?.Id == user2?.Id; }
        }

        private bool IsDateWithinReminderInterval(DateTime? date, int daysBefore) {
            return date.HasValue && date > DateTime.Now && date.Value.Subtract(new TimeSpan(daysBefore, 0, 0, 0)) < DateTime.Now;
        }
    }
}
