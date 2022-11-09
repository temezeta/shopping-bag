using shopping_bag.DTOs.Reminder;
using shopping_bag.Models;

namespace shopping_bag.Services {
    public interface IReminderService {

        Task<ServiceResponse<ReminderSettings>> SetReminder(long userId, ReminderSettingsDto settings);
        Task<ServiceResponse<ReminderSettingsDto>> SetListReminder(long userId, ReminderSettingsDto settings, long listId);
    }
}
