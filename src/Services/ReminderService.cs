using AutoMapper;
using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.DTOs.Reminder;
using shopping_bag.Models;
using shopping_bag.Models.User;

namespace shopping_bag.Services {
    public class ReminderService : IReminderService {

        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ReminderService(AppDbContext context, IUserService userService, IMapper mapper) {
            _context = context;
            _userService = userService;
            _mapper = mapper;
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
    }
}
