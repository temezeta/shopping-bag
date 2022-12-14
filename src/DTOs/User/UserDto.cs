using shopping_bag.Models.User;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using shopping_bag.DTOs.Office;
using shopping_bag.DTOs.Reminder;

namespace shopping_bag.DTOs.User {
    public class UserDto {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IEnumerable<UserRoleDto> UserRoles { get; set; } = new List<UserRoleDto>();
        public OfficeDto HomeOffice { get; set; }
        public bool Disabled { get; set; } = false;
        public ReminderSettingsDto ReminderSettings { get; set; }
        public IEnumerable<ListReminderSettingsDto> ListReminderSettings { get; set; }
    }
}
