using shopping_bag.DTOs.Reminder;
using shopping_bag.Models;

namespace shopping_bag.Services {
    public interface IReminderService {

        Task<ServiceResponse<ReminderSettings>> SetGlobalReminderSettings(long userId, ReminderSettingsDto settings);
        Task CreateRemindersForList(long listId, long officeId);
        Task<ServiceResponse<Reminder>> SetListReminder(long userId, ReminderSettingsDto settings, long listId);
    }
}
