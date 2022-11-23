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
            while (!stoppingToken.IsCancellationRequested) {

                // Wait until mid-night before running.
                var delay = new TimeSpan(24, 0, 0) - DateTime.Now.TimeOfDay;
                await Task.Delay(delay, stoppingToken);

                using var scope = _scopeFactory.CreateAsyncScope();
                var _reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();
                await _reminderService.SendReminders(stoppingToken);
            }
        }
    }
}
