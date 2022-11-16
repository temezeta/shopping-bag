using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.Models.User;
using shopping_bag.Services;
using System.Text;

namespace shopping_bag {
    public class ReminderBackgroundService : BackgroundService {

        private readonly IServiceScopeFactory _scopeFactory;

        public ReminderBackgroundService(IServiceScopeFactory scopeFactory) {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while(!stoppingToken.IsCancellationRequested) {

                // Wait until mid-night before running.
                var delay = new TimeSpan(24, 0, 0) - DateTime.Now.TimeOfDay;
                await Task.Delay(delay, stoppingToken);

                using var scope = _scopeFactory.CreateAsyncScope();
                var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var reminders = await _context.Reminders.Include(r => r.ShoppingList).Include(r => r.User).ThenInclude(u => u.ReminderSettings).ToListAsync(cancellationToken: stoppingToken);
                var remindersToSend = new Dictionary<User, List<string>>(); // Used to gather possible multiple reminders to be sent in one email.
                foreach(var reminder in reminders) {
                    var settings = reminder.User.ReminderSettings;
                    var list = reminder.ShoppingList;

                    // If list/user removed or both settings disabled
                    if (settings == null || reminder.ShoppingList.Removed || reminder.User.Removed || (settings.DueDateRemindersDisabled && settings.ExpectedRemindersDisabled)) {
                        reminder.User.Reminders.Remove(reminder);
                        continue;
                    }

                    // Due date reminders
                    if (reminder.DueDaysBefore.Any()) {
                        int daysBefore = reminder.DueDaysBefore.Max();
                        if (IsDateWithinReminderInterval(list.DueDate, daysBefore)) {
                            reminder.DueDaysBefore.Remove(daysBefore);
                            var msg = $"Due date for list {list.Name} is {(daysBefore == 1 ? "tomorrow" : $"in {daysBefore} days")} {list.DueDate.Value.ToString("dd.MM.yyyy")}. Items should be added before the due date.";
                            if(remindersToSend.TryGetValue(reminder.User, out List<string>? msgList)) {
                                msgList.Add(msg);
                            } else {
                                remindersToSend.Add(reminder.User, new List<string>() { msg });
                            }
                        }
                    }

                    // Expected delivery date reminders
                    if (reminder.ExpectedDaysBefore.Any()) {
                        int daysBefore = reminder.ExpectedDaysBefore.Max();
                        if (IsDateWithinReminderInterval(list.ExpectedDeliveryDate, daysBefore)) {
                            reminder.ExpectedDaysBefore.Remove(daysBefore);
                            var msg = $"Expected delivery date for list {list.Name} is {(daysBefore == 1 ? "tomorrow" : $"in {daysBefore} days")} {list.ExpectedDeliveryDate.Value.ToString("dd.MM.yyyy")}.";
                            if (remindersToSend.TryGetValue(reminder.User, out List<string>? msgList)) {
                                msgList.Add(msg);
                            } else {
                                remindersToSend.Add(reminder.User, new List<string>() { msg });
                            }
                        }
                    }

                    // Remove empty reminders
                    if (!reminder.DueDaysBefore.Any() && !reminder.ExpectedDaysBefore.Any()) {
                        reminder.User.Reminders.Remove(reminder);
                    }
                }
                // Send emails
                foreach(var kv in remindersToSend) {
                    var sb = new StringBuilder();
                    sb.AppendLine("TODO: Something about why this email was sent.");
                    sb.AppendJoin(Environment.NewLine, kv.Value); // List of reminders
                    sb.Append(Environment.NewLine);
                    sb.AppendLine("TODO: Something about how to turn off notifications");
                    _emailService.SendEmail(new Models.Email.Email() {
                        To = kv.Key.Email,
                        Subject = "TODO: subject",
                        Body = sb.ToString()
                    });
                }

                await _context.SaveChangesAsync(stoppingToken);
            }
        }
        private bool IsDateWithinReminderInterval(DateTime? date, int daysBefore) {
            return date.HasValue && date > DateTime.Now && date.Value.Subtract(new TimeSpan(daysBefore, 0, 0, 0)) < DateTime.Now;
        }
    }
}
