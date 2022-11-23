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
            var list = await _context.ShoppingLists.FirstOrDefaultAsync(l => l.Id == listId);
            if (list == null || (list.ExpectedDeliveryDate == null && list.DueDate == null)) {
                // No dates set, no need to make reminders.
                return;
            }
            var users = await _context.Users.Include(u => u.ReminderSettings).Include(u => u.ListReminderSettings).Include(u => u.Reminders).Where(u => u.OfficeId == officeId && !u.Removed).ToListAsync();
            foreach (var user in users) {
                var reminderSettings = user.ReminderSettings;
                if (reminderSettings == null || (reminderSettings.DueDateRemindersDisabled && reminderSettings.ExpectedRemindersDisabled)) {
                    continue;
                }
                if (user.Reminders.Any(r => r.ShoppingListId == listId)) {
                    // Skip if user already has reminders created on this list.
                    continue;
                }

                var listSettings = user.ListReminderSettings.FirstOrDefault(r => r.ShoppingListId == listId);

                // Generate list settings from globals if none exist.
                if(listSettings == null) {
                    listSettings = new ListReminderSettings() {
                        ReminderDaysBeforeDueDate = reminderSettings.ReminderDaysBeforeDueDate,
                        DueDateRemindersDisabled = reminderSettings.DueDateRemindersDisabled,
                        ExpectedRemindersDisabled = reminderSettings.ExpectedRemindersDisabled,
                        ReminderDaysBeforeExpectedDate = reminderSettings.ReminderDaysBeforeExpectedDate,
                        UserId = user.Id,
                        ShoppingListId = listId
                    };
                    user.ListReminderSettings.Add(listSettings);
                }

                var dueReminder = TrimInterval(listSettings.ReminderDaysBeforeDueDate, list.DueDate);
                var expectedReminder = TrimInterval(listSettings.ReminderDaysBeforeExpectedDate, list.ExpectedDeliveryDate);

                // If no future intervals found, dont create reminders.
                if (!dueReminder.Any() && !expectedReminder.Any()) {
                    continue;
                }

                var reminder = new Reminder() {
                    ShoppingListId = listId,
                    UserId = user.Id,
                    DueDaysBefore = dueReminder,
                    ExpectedDaysBefore = expectedReminder
                };
                user.Reminders.Add(reminder);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<ServiceResponse<User>> SetListReminder(long userId, ListReminderSettingsDto settings, long listId) {
            var resp = await _userService.GetUserById(userId);
            if(!resp.IsSuccess || resp.Data == null || resp.Data.Removed) {
                return new ServiceResponse<User>("Invalid user");
            }
            var user = resp.Data;
            var list = await _context.ShoppingLists.FirstOrDefaultAsync(l => l.Id == listId && !l.Removed);
            if (list == null) {
                return new ServiceResponse<User>("Invalid list");
            }

            if (!IsValidReminderInterval(settings.ReminderDaysBeforeDueDate)) {
                return new ServiceResponse<User>("Invalid due date reminder interval");
            }
            if (!IsValidReminderInterval(settings.ReminderDaysBeforeExpectedDate)) {
                return new ServiceResponse<User>("Invalid expected delivery date reminder interval");
            }

            // Save settings
            SaveListSettings(user, settings);

            var reminder = user.Reminders.FirstOrDefault(r => r.ShoppingListId == listId);
            var currentSettings = user.ListReminderSettings.FirstOrDefault(r => r.ShoppingListId == settings.ShoppingListId);

            // If both reminders are disabled, or both are empty, then remove the reminder.
            if ((settings.DueDateRemindersDisabled && settings.ExpectedRemindersDisabled) ||
                (settings.ReminderDaysBeforeDueDate.Count == 0 && settings.ReminderDaysBeforeExpectedDate.Count == 0)) {
                if (reminder != null) {
                    user.Reminders.Remove(reminder);
                    await _context.SaveChangesAsync();
                }
                return new ServiceResponse<User>(user);
            }

            var dueReminder = TrimInterval(settings.ReminderDaysBeforeDueDate, list.DueDate);
            var expectedReminder = TrimInterval(settings.ReminderDaysBeforeExpectedDate, list.ExpectedDeliveryDate);

            // If all dates are passed, remove this list's listsettings and reminder.
            if (!dueReminder.Any() && !expectedReminder.Any()) {
                if (reminder != null) {
                    user.Reminders.Remove(reminder);
                }
                user.ListReminderSettings.RemoveAll(r => r.ShoppingListId == listId);
                await _context.SaveChangesAsync();
                return new ServiceResponse<User>(user);
            }

            // Generate or update reminder
            if (reminder != null) {
                reminder.DueDaysBefore = settings.DueDateRemindersDisabled ? new List<int>() : dueReminder;
                reminder.ExpectedDaysBefore = settings.ExpectedRemindersDisabled ? new List<int>() : expectedReminder;
            } else {
                reminder = new Reminder() {
                    ShoppingListId = listId,
                    UserId = userId,
                    DueDaysBefore = settings.DueDateRemindersDisabled ? new List<int>() : dueReminder,
                    ExpectedDaysBefore = settings.ExpectedRemindersDisabled ? new List<int>() : expectedReminder
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

        // For example don't include 3 day if there's only 2 days till the date.
        private List<int> TrimInterval(List<int> interval, DateTime? targetDate) {
            return interval.Where(i => !targetDate.HasValue || targetDate.Value.Subtract(new TimeSpan(i, 0, 0, 0)) > DateTime.Now).ToList();
        }

        private void SaveListSettings(User user, ListReminderSettingsDto settings) {
            var currentSettings = user.ListReminderSettings.FirstOrDefault(r => r.ShoppingListId == settings.ShoppingListId);
            if (currentSettings == null) {
                user.ListReminderSettings.Add(new ListReminderSettings() {
                    DueDateRemindersDisabled = settings.DueDateRemindersDisabled,
                    ExpectedRemindersDisabled = settings.ExpectedRemindersDisabled,
                    ReminderDaysBeforeDueDate = settings.ReminderDaysBeforeDueDate,
                    ReminderDaysBeforeExpectedDate = settings.ReminderDaysBeforeExpectedDate,
                    UserId = user.Id,
                    ShoppingListId = settings.ShoppingListId
                });
            } else {
                currentSettings.DueDateRemindersDisabled = settings.DueDateRemindersDisabled;
                currentSettings.ExpectedRemindersDisabled = settings.ExpectedRemindersDisabled;
                currentSettings.ReminderDaysBeforeDueDate = settings.ReminderDaysBeforeDueDate;
                currentSettings.ReminderDaysBeforeExpectedDate = settings.ReminderDaysBeforeExpectedDate;
            }
        }

        public async Task SendReminders(CancellationToken stoppingToken) {
            var reminders = await _context.Reminders.Include(r => r.ShoppingList).Include(r => r.User).ThenInclude(u => u.ReminderSettings)
                .Include(r => r.User).ThenInclude(r => r.ListReminderSettings).ToListAsync(cancellationToken: stoppingToken);
            var remindersToSend = new Dictionary<User, List<string>>(new UserEqualityComparer()); // Used to gather possible multiple reminders to be sent in one email.
            foreach (var reminder in reminders) {
                try {
                    var settings = reminder.User.ReminderSettings;
                    var list = reminder.ShoppingList;
                    var listSettings = reminder.User.ListReminderSettings.FirstOrDefault(r => r.ShoppingListId == list.Id);

                    // If list or user removed, remove reminder and settings.
                    if (reminder.ShoppingList.Removed || reminder.User.Removed) {
                        reminder.User.Reminders.Remove(reminder);
                        reminder.User.ListReminderSettings.RemoveAll(r => r.ShoppingListId == list.Id);
                        await _context.SaveChangesAsync(stoppingToken);
                        continue;
                    }

                    // If both settings null or both disabled, remove reminder.
                    if ((settings == null || (settings.DueDateRemindersDisabled && settings.ExpectedRemindersDisabled)) &&
                            (listSettings == null || (listSettings.DueDateRemindersDisabled && listSettings.ExpectedRemindersDisabled))) {
                        reminder.User.Reminders.Remove(reminder);
                        await _context.SaveChangesAsync(stoppingToken);
                        continue;
                    }

                    HandleDueDateReminder(reminder, remindersToSend);
                    HandleExpectedDateReminder(reminder, remindersToSend);

                    // Remove empty reminders
                    if (!reminder.DueDaysBefore.Any() && !reminder.ExpectedDaysBefore.Any()) {
                        reminder.User.Reminders.Remove(reminder);
                    }

                    await _context.SaveChangesAsync(stoppingToken);
                } catch(Exception) {
                    continue;
                }
            }
            // Send emails
            SendReminderEmails(remindersToSend);
        }

        private void HandleDueDateReminder(Reminder reminder, Dictionary<User, List<string>> remindersToSend) {
            var list = reminder.ShoppingList;
            if (reminder.DueDaysBefore.Any()) {
                int daysBefore = reminder.DueDaysBefore.Max();
                if (IsDateWithinReminderInterval(list.DueDate, daysBefore)) {
                    reminder.DueDaysBefore.Remove(daysBefore);
                    var msg = string.Format(StaticConfig.EmailDueDateReminderFormat, list.Name, (daysBefore == 1 ? "tomorrow" : $"in {daysBefore} days"), list.DueDate.Value.ToString("dd.MM.yyyy"));
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
                    var msg = string.Format(StaticConfig.EmailExpectedDateReminderFormat, list.Name, (daysBefore == 1 ? "tomorrow" : $"in {daysBefore} days"), list.ExpectedDeliveryDate.Value.ToString("dd.MM.yyyy"));
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
                sb.Append(StaticConfig.EmailReminderIntro);
                sb.Append("<br>");
                sb.AppendJoin("<br>", kv.Value); // List of reminders
                sb.Append("<br>");
                sb.AppendLine(StaticConfig.EmailReminderTurnOffEmails);
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
