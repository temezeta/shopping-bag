using shopping_bag.Services;

namespace shopping_bag.Config
{
    public static class ServiceInjector
    {
        public static void AddServices(this IServiceCollection services)
        {
            // Add services for dependency injection here
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOfficeService, OfficeService>();
            services.AddScoped<IShoppingListService, ShoppingListService>();
            services.AddScoped<IReminderService, ReminderService>();
            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddHostedService<ReminderBackgroundService>();
        }
    }
}
