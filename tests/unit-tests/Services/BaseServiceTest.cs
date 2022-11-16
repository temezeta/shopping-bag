using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using shopping_bag.Config;

namespace shopping_bag_unit_tests.Services
{
    public class BaseServiceTest : TestDataProvider
    {
        public BaseServiceTest() : base() {
            UnitTestHelper.SetupStaticConfig();
        }

        protected AppDbContext GetDatabase()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

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
