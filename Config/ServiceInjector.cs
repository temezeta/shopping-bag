using shopping_bag.Services;
using shopping_bag.Stores;

namespace shopping_bag.Config
{
    public static class ServiceInjector
    {
        public static void AddServices(this IServiceCollection services)
        {
            // Add services for dependency injection here
            // Services
            services.AddScoped<IUserService, UserService>();
            // Stores
            services.AddScoped<IUserStore, DummyUserStore>();
        }
    }
}
