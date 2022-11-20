using shopping_bag.DTOs.Reminder;
using shopping_bag.Models;
using shopping_bag.Models.User;

namespace shopping_bag.Services {
    public interface IReminderService {

        Task<ServiceResponse<ReminderSettings>> SetGlobalReminderSettings(long userId, ReminderSettingsDto settings);
        Task CreateRemindersForList(long listId, long officeId);
        Task<ServiceResponse<User>> SetListReminder(long userId, ReminderSettingsDto settings, long listId);
    }
}
