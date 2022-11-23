using System.ComponentModel.DataAnnotations;

namespace shopping_bag.Models {
    public class ReminderSettings {
        [Key]
        public long UserId { get; set; }
        public User.User User { get; set; }
        public bool AllRemindersDisabled { get; set; }
        public bool DueDateRemindersDisabled { get; set; }
        public bool ExpectedRemindersDisabled { get; set; }
        public List<int> ReminderDaysBeforeDueDate { get; set; } = new List<int>();
        public List<int> ReminderDaysBeforeExpectedDate { get; set; } = new List<int>();
    }
}
