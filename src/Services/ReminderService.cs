using AutoMapper;
using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.DTOs.Reminder;
using shopping_bag.Models;

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
            user.ReminderSettings = _mapper.Map<ReminderSettings>(settings);
            await _context.SaveChangesAsync();
            return new ServiceResponse<ReminderSettings>(user.ReminderSettings);
        }

        public async Task CreateRemindersForList(long listId, long officeId) {
            var users = await _context.Users.Include(u => u.ReminderSettings).Where(u => u.OfficeId == officeId).ToListAsync();
            foreach (var user in users) {
                var reminderSettings = user.ReminderSettings;
                if (reminderSettings == null || (reminderSettings.DueDateRemindersDisabled && reminderSettings.ExpectedRemindersDisabled)) {
                    continue;
                }
                var reminder = new Reminder() {
                    ShoppingListId = listId,
                    UserId = user.Id,
                    DueDaysBefore = reminderSettings.ReminderDaysBeforeDueDate,
                    ExpectedDaysBefore = reminderSettings.ReminderDaysBeforeExpectedDate
                };
                reminderSettings.Reminders.Add(reminder);
            }
            await _context.SaveChangesAsync();
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
