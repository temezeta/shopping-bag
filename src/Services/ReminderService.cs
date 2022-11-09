using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.DTOs.Reminder;
using shopping_bag.Models;
using System.Runtime.CompilerServices;

namespace shopping_bag.Services {
    public class ReminderService : IReminderService {

        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        public ReminderService(AppDbContext context, IUserService userService) {
            _context = context;
            _userService = userService;
        }

        /*public async Task<Reminder> GetReminder(long userId, long listId) {
            return await _context.Reminders.FirstOrDefaultAsync(r => r.UserId == userId && r.ShoppingListId == listId);
        }*/

        public async Task<ServiceResponse<ReminderSettings>> SetReminder(long userId, ReminderSettingsDto settings) {
            var resp = await _userService.GetUserById(userId);
            if(!resp.IsSuccess) {
                return new ServiceResponse<ReminderSettings>("Invalid user");
            }
            var user = resp.Data;
            user.ReminderSettings.ReminderDaysBeforeDueDate = settings.ReminderDaysBeforeDueDate;
            user.ReminderSettings.ReminderDaysBeforeExpectedDate = settings.ReminderDaysBeforeExpectedDate;
            user.ReminderSettings.DueDateRemindersDisabled = settings.DueDateRemindersDisabled;
            user.ReminderSettings.ExpectedRemindersDisabled = settings.ExpectedRemindersDisabled;
            await _context.SaveChangesAsync();
            return new ServiceResponse<ReminderSettings>(user.ReminderSettings);
        }

        public async Task<ServiceResponse<ReminderSettingsDto>> SetListReminder(long userId, ReminderSettingsDto settings, long listId) {
            var resp = await _userService.GetUserById(userId);
            if (!resp.IsSuccess) {
                return new ServiceResponse<ReminderSettingsDto>("Invalid user");
            }
            if(!(await _context.ShoppingLists.AnyAsync(l => l.Id == listId))) {
                return new ServiceResponse<ReminderSettingsDto>("Invalid list");
            }
            var user = resp.Data;
            var reminder = await _context.Reminders.FirstOrDefaultAsync(r => r.UserId == userId && r.ShoppingListId == listId);
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
                user.ReminderSettings.Reminders.Add(reminder);
            }
            await _context.SaveChangesAsync();
            return new ServiceResponse<ReminderSettingsDto>(new ReminderSettingsDto() { 
                DueDateRemindersDisabled = reminder.DueDaysBefore.Any(),
                ExpectedRemindersDisabled = reminder.ExpectedDaysBefore.Any(),
                ReminderDaysBeforeDueDate = reminder.DueDaysBefore,
                ReminderDaysBeforeExpectedDate = reminder.ExpectedDaysBefore
            });
        }
    }
}
