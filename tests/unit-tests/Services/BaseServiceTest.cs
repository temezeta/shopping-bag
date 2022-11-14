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
        protected readonly List<ShoppingList> ShoppingLists;
        protected readonly List<Item> Items;

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
            ShoppingLists = new List<ShoppingList>()
            {
                new ShoppingList() { Id = 1, Name = "Test list", DueDate = DateTime.Now.AddMinutes(10), Ordered = false, ListDeliveryOffice = Offices[1] },
                new ShoppingList() { Id = 2, Name = "Test list 2", DueDate = DateTime.Now.AddMinutes(-10), Ordered = false, ListDeliveryOffice = Offices[1] },
                new ShoppingList() { Id = 3, Name = "Test list 3", Ordered = false, ListDeliveryOffice = Offices[1] },
                new ShoppingList() { Id = 4, Name = "Test list 2", DueDate = DateTime.Now.AddMinutes(-10), Ordered = true, ListDeliveryOffice = Offices[1] },
                new ShoppingList() { Id = 5, Name = "Test list 5", DueDate = DateTime.Now.AddMinutes(-10), Ordered = true, ListDeliveryOffice = Offices[1], Removed = true}
            };
            Items = new List<Item>()
            {
                new Item() { Id = 1, Name = "Own item in list", UserId = 1, ShoppingListId = ShoppingLists[0].Id, ShoppingList = ShoppingLists[0] },
                new Item() { Id = 2, Name = "Own item in list 2", UserId = 1, ShoppingListId = ShoppingLists[0].Id, ShoppingList = ShoppingLists[0] },
                new Item() { Id = 3, Name = "Others item in list", UserId = null, ShoppingListId = ShoppingLists[0].Id, ShoppingList = ShoppingLists[0] },
                new Item() { Id = 4, Name = "Item in dueDatePassedList", UserId = 1, ShoppingListId = ShoppingLists[1].Id, ShoppingList = ShoppingLists[1] },
                new Item() { Id = 5, Name = "Item in orderedList", UserId = 1, ShoppingListId = ShoppingLists[3].Id, ShoppingList = ShoppingLists[3] },
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
            context.ShoppingLists.AddRange(ShoppingLists);
            context.SaveChanges();
            context.Items.AddRange(Items);
            context.SaveChanges();
        }
    }
}
