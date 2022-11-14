using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.Models.User;
using shopping_bag.Models;

namespace shopping_bag_unit_tests.Services
{
    public class BaseServiceTest
    {
        private readonly List<Office> Offices;
        private readonly List<UserRole> UserRoles;
        private readonly List<User> Users;

        public BaseServiceTest()
        {
            Offices = new List<Office>()
            {
                new Office() { Id = 1, Name = "Tampere" }
            };
            UserRoles = new List<UserRole>()
            {
                new UserRole() { RoleId = 1, RoleName = "User" },
                new UserRole() { RoleId = 2, RoleName = "Admin" }
            };
            Users = new List<User>()
            {
                new User() { Id = 1, UserRoles = new List<UserRole>() { UserRoles[0] }, Email = "", FirstName = "Normal", LastName = "User", PasswordHash = Array.Empty<byte>(), PasswordSalt = Array.Empty<byte>(), HomeOffice = Offices[0] },
                new User() { Id = 2, UserRoles = new List<UserRole>() { UserRoles[1] }, Email = "", FirstName = "Admin", LastName = "User", PasswordHash = Array.Empty<byte>(), PasswordSalt = Array.Empty<byte>(), HomeOffice = Offices[0] }
            };
        }

        protected AppDbContext GetDatabase()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            SetupDb(context);

            return context;
        }

        private void SetupDb(AppDbContext context)
        {
            context.RemoveRange(context.ShoppingLists.ToList());
            context.RemoveRange(context.Items.ToList());
            context.RemoveRange(context.Offices.ToList());
            context.RemoveRange(context.Users.ToList());
            context.RemoveRange(context.UserRoles.ToList());
            context.SaveChanges();

            context.Offices.AddRange(Offices);
            context.SaveChanges();
            context.UserRoles.AddRange(UserRoles);
            context.SaveChanges();
            context.Users.AddRange(Users);
            context.SaveChanges();
        }
    }
}
