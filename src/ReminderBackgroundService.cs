using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;

namespace shopping_bag {
    public class ReminderBackgroundService : BackgroundService {

        private readonly IServiceScopeFactory _scopeFactory;

        public ReminderBackgroundService(IServiceScopeFactory scopeFactory) {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while(!stoppingToken.IsCancellationRequested) {
                using var scope = _scopeFactory.CreateAsyncScope();
                var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var users = await _context.Users.Include(u => u.ReminderSettings).ThenInclude(r => r.Reminders).ToListAsync(cancellationToken: stoppingToken);
                var lists = await _context.ShoppingLists.Where(l => !l.Removed && (l.DueDate > DateTime.Now || l.ExpectedDeliveryDate > DateTime.Now))
                    .ToListAsync(cancellationToken: stoppingToken);
                foreach (var list in lists) {
                    foreach (var user in users) {
                        var reminderSettings = user.ReminderSettings;
                        if (reminderSettings == null || (reminderSettings.DueDateRemindersDisabled && reminderSettings.ExpectedRemindersDisabled)) {
                            continue;
                        }
                        var reminders = reminderSettings.Reminders.Where(r => r.ShoppingListId == list.Id).ToList();
                        foreach (var reminder in reminders) {
                            if (reminder.DueDaysBefore.Any()) {
                                int daysBefore = reminder.DueDaysBefore.Max();
                                if (list.DueDate.HasValue && list.DueDate > DateTime.Now && list.DueDate.Value.Subtract(new TimeSpan(daysBefore, 0, 0, 0)) < DateTime.Now) {
                                    reminder.DueDaysBefore.Remove(daysBefore);
                                    // Send notification here
                                    Console.WriteLine("Sent due reminder!");
                                }
                            }
                            if (reminder.ExpectedDaysBefore.Any()) {
                                int daysBefore = reminder.ExpectedDaysBefore.Max();
                                if (list.ExpectedDeliveryDate.HasValue && list.ExpectedDeliveryDate > DateTime.Now && list.ExpectedDeliveryDate.Value.Subtract(new TimeSpan(daysBefore, 0, 0, 0)) < DateTime.Now) {
                                    reminder.ExpectedDaysBefore.Remove(daysBefore);
                                    // Send notification here
                                    Console.WriteLine("Sent expected reminder!");
                                }
                            }
                            if (!reminder.DueDaysBefore.Any() && !reminder.ExpectedDaysBefore.Any()) {
                                reminderSettings.Reminders.Remove(reminder);
                                _context.Reminders.Remove(reminder);
                                Console.WriteLine("Removed reminders as none are left!");
                            }
                            await _context.SaveChangesAsync(stoppingToken);
                        }
                    }
                }

                await Task.Delay(new TimeSpan(1, 0, 0), stoppingToken);
            }
        }
    }
}
