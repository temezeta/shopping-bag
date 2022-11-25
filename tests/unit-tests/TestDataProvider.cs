using shopping_bag.Models.User;
using shopping_bag.Models;
using shopping_bag.Migrations;

namespace shopping_bag_unit_tests
{
    public class TestDataProvider
    {
        public readonly Office TestOffice, ListOffice, RemovedOffice;
        public readonly UserRole NormalRole, AdminRole;
        public readonly User NormalUser, AdminUser, DisabledUser, UnverifiedUser;
        public readonly ShoppingList NormalList, DueDatePassedList, NotStartedList, OrderedList, RemovedList;
        public readonly Item OwnItemInList, OwnItem2InList, OthersItemInList, ItemInDueDatePassedList, ItemInOrderedList;

        public readonly List<Office> Offices;
        public readonly List<UserRole> UserRoles;
        public readonly List<User> Users;
        public readonly List<ShoppingList> ShoppingLists;
        public readonly List<Item> Items;

        public TestDataProvider()
        {
            #region Offices
            TestOffice = new Office() { Id = 1, Name = "Tampere" };
            ListOffice = new Office() { Id = 2, Name = "Helsinki" };
            RemovedOffice = new Office() { Id = 3, Name = "Karibia", Removed = true };
            
            Offices = new List<Office>() { TestOffice, ListOffice, RemovedOffice };
            #endregion

            #region UserRoles
            NormalRole = new UserRole() { RoleId = 1, RoleName = "User" };
            AdminRole = new UserRole() { RoleId = 2, RoleName = "Admin" };
            
            UserRoles = new List<UserRole>() { NormalRole, AdminRole };
            #endregion

            #region Users
            NormalUser = new User() { Id = 1, UserRoles = new List<UserRole>() { UserRoles[0] }, Email = "regular@huld.io", FirstName = "Normal", LastName = "User", PasswordHash = Array.Empty<byte>(), PasswordSalt = Array.Empty<byte>(), HomeOffice = Offices[0], VerifiedAt = DateTime.Now };
            AdminUser = new User() { Id = 2, UserRoles = new List<UserRole>() { UserRoles[1] }, Email = "admin@huld.io", FirstName = "Admin", LastName = "User", PasswordHash = Array.Empty<byte>(), PasswordSalt = Array.Empty<byte>(), HomeOffice = Offices[0], VerifiedAt = DateTime.Now };
            DisabledUser = new User() { Id = 3, UserRoles = new List<UserRole>() { UserRoles[1] }, Email = "admin2@huld.io", FirstName = "Admin", LastName = "User 2", PasswordHash = Array.Empty<byte>(), PasswordSalt = Array.Empty<byte>(), HomeOffice = Offices[0], Disabled = true };
            UnverifiedUser = new User() { Id = 4, UserRoles = new List<UserRole>() { UserRoles[1] }, Email = "unverified@huld.io", FirstName = "Unverified", LastName = "User", PasswordHash = Array.Empty<byte>(), PasswordSalt = Array.Empty<byte>(), HomeOffice = Offices[0]};

            Users = new List<User>() { NormalUser, AdminUser, DisabledUser, UnverifiedUser };
            #endregion

            #region ShoppingLists
            NormalList = new ShoppingList() { Id = 1, Name = "Test list", DueDate = DateTime.Now.AddMinutes(10), Ordered = false, ListDeliveryOffice = Offices[1] };
            DueDatePassedList = new ShoppingList() { Id = 2, Name = "Test list 2", DueDate = DateTime.Now.AddMinutes(-10), Ordered = false, ListDeliveryOffice = Offices[1] };
            NotStartedList = new ShoppingList() { Id = 3, Name = "Test list 3", Ordered = false, ListDeliveryOffice = Offices[1] };
            OrderedList = new ShoppingList() { Id = 4, Name = "Test list 2", DueDate = DateTime.Now.AddMinutes(-10), Ordered = true, ListDeliveryOffice = Offices[1] };
            RemovedList = new ShoppingList() { Id = 5, Name = "Test list 5", DueDate = DateTime.Now.AddMinutes(-10), Ordered = true, ListDeliveryOffice = Offices[1], Removed = true };
            
            ShoppingLists = new List<ShoppingList>() { NormalList, DueDatePassedList, NotStartedList, OrderedList, RemovedList };
            #endregion

            #region Items
            OwnItemInList = new Item() { Id = 1, Name = "Own item in list", UserId = 1, ShoppingListId = ShoppingLists[0].Id, ShoppingList = ShoppingLists[0] };
            OwnItem2InList = new Item() { Id = 2, Name = "Own item in list 2", UserId = 1, ShoppingListId = ShoppingLists[0].Id, ShoppingList = ShoppingLists[0] };
            OthersItemInList = new Item() { Id = 3, Name = "Others item in list", UserId = null, ShoppingListId = ShoppingLists[0].Id, ShoppingList = ShoppingLists[0] };
            ItemInDueDatePassedList = new Item() { Id = 4, Name = "Item in dueDatePassedList", UserId = 1, ShoppingListId = ShoppingLists[1].Id, ShoppingList = ShoppingLists[1] };
            ItemInOrderedList = new Item() { Id = 5, Name = "Item in orderedList", UserId = 1, ShoppingListId = ShoppingLists[3].Id, ShoppingList = ShoppingLists[3] };

            Items = new List<Item>() { OwnItemInList, OwnItem2InList, OthersItemInList, ItemInDueDatePassedList, ItemInOrderedList };
            #endregion
        }
    }
}
