namespace shopping_bag.DTOs.Reminder {
    public class ReminderSettingsDto {
        public bool DueDateRemindersDisabled { get; set; }
        public bool ExpectedRemindersDisabled { get; set; }
        public List<int> ReminderDaysBeforeDueDate { get; set; } = new List<int>();
        public List<int> ReminderDaysBeforeExpectedDate { get; set; } = new List<int>();
    }
}
