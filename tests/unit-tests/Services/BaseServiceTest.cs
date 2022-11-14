using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.Models.User;
using shopping_bag.Models;

namespace shopping_bag_unit_tests.Services
{
    public class BaseServiceTest
    {
        protected readonly List<Office> Offices;
        protected readonly List<UserRole> UserRoles;
        protected readonly List<User> Users;

        public BaseServiceTest()
        {
            Offices = new List<Office>()
            {
                new Office() { Id = 1, Name = "Tampere" },
                new Office() { Id = 2, Name = "Helsinki"}
            };
            UserRoles = new List<UserRole>()
            {
                new UserRole() { RoleId = 1, RoleName = "User" },
                new UserRole() { RoleId = 2, RoleName = "Admin" }
            };
            Users = new List<User>()
            {
                new User() { Id = 1, UserRoles = new List<UserRole>() { UserRoles[0] }, Email = "regular@huld.io", FirstName = "Normal", LastName = "User", PasswordHash = Array.Empty<byte>(), PasswordSalt = Array.Empty<byte>(), HomeOffice = Offices[0] },
                new User() { Id = 2, UserRoles = new List<UserRole>() { UserRoles[1] }, Email = "admin@huld.io", FirstName = "Admin", LastName = "User", PasswordHash = Array.Empty<byte>(), PasswordSalt = Array.Empty<byte>(), HomeOffice = Offices[0] },
                new User() { Id = 3, UserRoles = new List<UserRole>() { UserRoles[1] }, Email = "admin2@huld.io", FirstName = "Admin", LastName = "User 2", PasswordHash = Array.Empty<byte>(), PasswordSalt = Array.Empty<byte>(), HomeOffice = Offices[0], Removed = true }
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
