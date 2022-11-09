using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using shopping_bag.Config;
using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Models;
using shopping_bag.Models.User;
using shopping_bag.Services;
using shopping_bag.Utility;

namespace shopping_bag_unit_tests.Services {
    public class ShoppingListServiceTests {

        private readonly AppDbContext _context;
        private readonly ShoppingListService _sut;

        private readonly Office testOffice, listOffice;
        private readonly User normalUser, adminUser;
        private readonly ShoppingList normalList, dueDatePassedList, notStartedList, orderedList;
        private readonly Item ownItemInList, ownItem2InList, othersItemInList, itemInDueDatePassedList, itemInOrderedList;
        private readonly UserRole normalRole, adminRole;

        public ShoppingListServiceTests() {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();

            var profile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            var mapper = new Mapper(configuration);
            _sut = new ShoppingListService(_context, mapper);

            testOffice = new Office() { Id = 1, Name = "Tampere" };
            listOffice = new Office() { Id = 2, Name = "Helsinki" };

            normalRole = new UserRole() { RoleId = 1, RoleName = "User" };
            adminRole = new UserRole() { RoleId = 2, RoleName = "Admin" };
            normalUser = new User() { Id = 1, UserRoles = new List<UserRole>() { normalRole }, Email = "", FirstName = "", LastName = "", PasswordHash = Array.Empty<byte>(), PasswordSalt = Array.Empty<byte>() };
            adminUser = new User() { Id = 2, UserRoles = new List<UserRole>() { adminRole }, Email = "", FirstName = "", LastName = "", PasswordHash = Array.Empty<byte>(), PasswordSalt = Array.Empty<byte>() };

            normalList = new ShoppingList() { Id = 1, Name = "Test list", DueDate = DateTime.Now.AddMinutes(10), Ordered = false, ListDeliveryOffice = listOffice };
            dueDatePassedList = new ShoppingList() { Id = 2, Name = "Test list 2", DueDate = DateTime.Now.AddMinutes(-10), Ordered = false, ListDeliveryOffice = listOffice };
            notStartedList = new ShoppingList() { Id = 3, Name = "Test list 3", Ordered = false, ListDeliveryOffice = listOffice };
            orderedList = new ShoppingList() { Id = 4, Name = "Test list 2", DueDate = DateTime.Now.AddMinutes(-10), Ordered = true, ListDeliveryOffice = listOffice };

            ownItemInList = new Item() { Id = 1, Name = "Own item in list", UserId = 1, ShoppingListId = normalList.Id, ShoppingList = normalList };
            ownItem2InList = new Item() { Id = 2, Name = "Own item in list 2", UserId = 1, ShoppingListId = normalList.Id, ShoppingList = normalList };
            othersItemInList = new Item() { Id = 3, Name = "Others item in list", UserId = null, ShoppingListId = normalList.Id, ShoppingList = normalList };
            itemInDueDatePassedList = new Item() { Id = 4, Name = "Item in dueDatePassedList", UserId = 1, ShoppingListId = dueDatePassedList.Id, ShoppingList = dueDatePassedList };
            itemInOrderedList = new Item() { Id = 5, Name = "Item in orderedList", UserId = 1, ShoppingListId = orderedList.Id, ShoppingList = orderedList };
        }

        #region AddShoppingList Tests
        [Fact]
        public async Task AddShoppingList_ListWithOnlyNameAndOffice_ShoppingListAdded()
        {
            SetupDb();
            var result = await _sut.AddShoppingList(new AddShoppingListDto() { Name = "Office supplies", OfficeId = 1, UserId = 3});
            Assert.True(result.IsSuccess);
        }
        #endregion

        #region GetShoppingListById Tests
        [Fact]
        public async Task GetShoppingListById_ValidId_ShoppingListReturned() {
            SetupDb();
            var response = await _sut.GetShoppingListById(normalList.Id);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task GetShoppingListById_InvalidId_ErrorReturned()
        {
            SetupDb();
            var response = await _sut.GetShoppingListById(827);
            Assert.False(response.IsSuccess);
            Assert.Equal("Invalid shoppingListId", response.Error);
        }
        #endregion

        #region GetShoppingListsByOffice Tests
        [Fact]
        public async Task GetShoppingListsByOffice_InvalidOfficeId_ErrorReturned()
        {
            SetupDb();
            var result = await _sut.GetShoppingListsByOffice(14);
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid officeId", result.Error);
        }

        [Fact]
        public async Task GetShoppingListsByOffice_ValidOfficeId_ListsReturned()
        {
            SetupDb();
            var list = await _sut.AddShoppingList(new AddShoppingListDto() { Name = "Office supplies", OfficeId = 1, UserId = 3 });
            Assert.True(list.IsSuccess);
            var result = await _sut.GetShoppingListsByOffice(1);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Data);
        }
        #endregion

        #region OrderShoppingList Tests
        [Fact]
        public async Task OrderShoppingList_ValidShoppingList_ShoppingListOrderedOnlyOnce()
        {
            SetupDb();
            var response = await _sut.OrderShoppingList(1);
            Assert.True(response.IsSuccess);
            // Order the same again
            response = await _sut.OrderShoppingList(1);
            Assert.False(response.IsSuccess);
            Assert.Equal("Shoppinglist is already ordered", response.Error);
        }

        #endregion

        #region ModifyShoppingList test
        [Fact]
        public async Task ModifyShoppingList_ValidOfficeId_ListModified()
        {
            SetupDb();
            var list = await _sut.AddShoppingList(new AddShoppingListDto() { Name = "Office supplies", OfficeId = 1, UserId = 3 });
            Assert.True(list.IsSuccess);

            var result = await _sut.ModifyShoppingList(new ModifyShoppingListDto() { Name = "Office supplies 2"}, list.Data.Id);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ModifyShoppingList_ActiveListWithNameAlreadyExists_ListNotModified()
        {
            SetupDb();
            var list = await _sut.AddShoppingList(new AddShoppingListDto() { Name = "Office supplies", OfficeId = 1, UserId = 3 });
            var list2 = await _sut.AddShoppingList(new AddShoppingListDto() { Name = "Office supplies 2", OfficeId = 1, UserId = 3 });
            Assert.True(list.IsSuccess && list2.IsSuccess);

            var result = await _sut.ModifyShoppingList(new ModifyShoppingListDto() { Name = "Office supplies 2" }, list.Data.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("Active shopping list with that name already exists.", result.Error);
        }
        #endregion

        #region RemoveShoppingList Test
        [Fact]
        public async Task RemoveShoppingList_RemoveListAndTryRemoveAgain_ListRemoved()
        {
            SetupDb();
            var remove = await _sut.RemoveShoppingList(normalList.Id);
            Assert.True(remove.IsSuccess);

            var removeAgain = await _sut.RemoveShoppingList(normalList.Id);
            Assert.False(removeAgain.IsSuccess);
            Assert.Equal("Invalid shoppingListId", removeAgain.Error);
        }

        #endregion



        #region AddItemToShoppingList Tests
        [Fact]
        public async Task AddItem_ValidItem_ItemAdded() {
            SetupDb();

            // Ensure service result success.
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = normalList.Id, Name = "Test item" });
            Assert.True(result.IsSuccess);
            Assert.Equal(normalList.Id, result.Data.ShoppingListId);

            // Ensure item added to list
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Id == result.Data.ShoppingListId);
            Assert.NotNull(list);
            Assert.NotNull(list.Items);
            Assert.Single(list.Items.Where(i => i.Id == result.Data.Id));
            Assert.Equal("Test item", list.Items.FirstOrDefault(i => i.Id == result.Data.Id)?.Name);
        }

        [Fact]
        public async Task AddItem_InvalidListId_ReturnsError() {
            SetupDb();

            // Ensure service result error
            var itemName = "Test item not added";
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = -1, Name = itemName });
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid shoppingListId", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any(i => i.Name == itemName));
            Assert.Null(list);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(null, "")]
        public async Task AddItem_MissingNameOrUrl_ReturnsError(string name, string url) {
            SetupDb();

            // Ensure service result error
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = normalList.Id, Name = name, Url = url });
            Assert.False(result.IsSuccess);
            Assert.Equal("Item url or name must be given", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any(i => i.Name == name && i.Url == url));
            Assert.Null(list);
        }

        [Fact]
        public async Task AddItem_OrderedList_ReturnsError() {
            SetupDb();

            // Ensure service result error
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = orderedList.Id, Name = "Test item" });
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list already ordered", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any(i => i.Name == "Test item"));
            Assert.Null(list);
        }

        [Fact]
        public async Task AddItem_DueDatePassedList_ReturnsError() {
            SetupDb();

            // Ensure service result error
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = dueDatePassedList.Id, Name = "Test item" });
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list due date passed", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any(i => i.Name == "Test item"));
            Assert.Null(list);
        }

        [Fact]
        public async Task AddItem_DuplicateUrl_ReturnsError() {
            SetupDb();

            // Ensure service result ok
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = normalList.Id, Url = "http://example.com" });
            Assert.True(result.IsSuccess);

            // Ensure service result error
            result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = normalList.Id, Url = "http://example.com" });
            Assert.False(result.IsSuccess);
            Assert.Equal("Item with same url already in list", result.Error);

            // Ensure only first item added to list.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Count(i => i.Url == "http://example.com") == 1);
            Assert.NotNull(list);
        }
        #endregion

        #region RemoveItemFromShoppingList Tests
        [Fact]
        public async Task RemoveItem_UserValidListOwnItem_ItemRemoved() {
            SetupDb();

            var result = await _sut.RemoveItemFromShoppingList(normalUser, ownItemInList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == ownItemInList.Id);
            Assert.Null(item);
        }

        [Fact]
        public async Task RemoveItem_UserValidListNotOwnItem_ItemNotRemoved() {
            SetupDb();

            var result = await _sut.RemoveItemFromShoppingList(normalUser, othersItemInList.Id);
            Assert.False(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == othersItemInList.Id);
            Assert.NotNull(item);
        }

        [Fact]
        public async Task RemoveItem_AdminValidListNotOwnItem_ItemRemoved() {
            SetupDb();

            var result = await _sut.RemoveItemFromShoppingList(adminUser, othersItemInList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == othersItemInList.Id);
            Assert.Null(item);
        }

        [Fact]
        public async Task RemoveItem_UserDueDatePassedList_ItemNotRemoved() {
            SetupDb();

            var result = await _sut.RemoveItemFromShoppingList(normalUser, itemInDueDatePassedList.Id);
            Assert.False(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == itemInDueDatePassedList.Id);
            Assert.NotNull(item);
        }

        [Fact]
        public async Task RemoveItem_AdminDueDatePassedList_ItemRemoved() {
            SetupDb();

            var result = await _sut.RemoveItemFromShoppingList(adminUser, itemInDueDatePassedList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == itemInDueDatePassedList.Id);
            Assert.Null(item);
        }
        #endregion

        #region ModifyItem Tests
        [Fact]
        public async Task ModifyItem_UserValidItem_ItemModified() {
            SetupDb();

            var result = await _sut.ModifyItem(normalUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, ownItemInList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal("Test item", item.Name);
            Assert.Equal("New url", item.Url);
        }
        [Fact]
        public async Task ModifyItem_AdminNotOwnItem_ItemModified() {
            SetupDb();

            var result = await _sut.ModifyItem(adminUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, othersItemInList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal("Test item", item.Name);
            Assert.Equal("New url", item.Url);
        }

        [Fact]
        public async Task ModifyItem_UserNotOwnItem_ItemNotModified() {
            SetupDb();

            var result = await _sut.ModifyItem(normalUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, othersItemInList.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("You can only modify items you have added", result.Error);
        }

        [Fact]
        public async Task ModifyItem_InvalidItemId_ItemNotModified() {
            SetupDb();

            var result = await _sut.ModifyItem(normalUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, -1);
            Assert.False(result.IsSuccess);
            Assert.Equal("Item doesn't exist.", result.Error);
        }

        [Fact]
        public async Task ModifyItem_UserOrderedList_ItemNotModified() {
            SetupDb();

            var result = await _sut.ModifyItem(normalUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, itemInOrderedList.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list already ordered", result.Error);
        }

        [Fact]
        public async Task ModifyItem_AdminOrderedList_ItemModified() {
            SetupDb();

            var result = await _sut.ModifyItem(adminUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, itemInOrderedList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal("Test item", item.Name);
            Assert.Equal("New url", item.Url);
        }

        [Fact]
        public async Task ModifyItem_UserDueDatePassedList_ItemNotModified() {
            SetupDb();

            var result = await _sut.ModifyItem(normalUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, itemInDueDatePassedList.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list due date passed", result.Error);
        }

        [Fact]
        public async Task ModifyItem_AdminDueDatePassedList_ItemModified() {
            SetupDb();

            var result = await _sut.ModifyItem(adminUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, itemInDueDatePassedList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal("Test item", item.Name);
            Assert.Equal("New url", item.Url);
        }

        [Fact]
        public async Task ModifyItem_DuplicateUrl_ItemNotModified() {
            SetupDb();

            var result = await _sut.ModifyItem(normalUser, new ModifyItemDto() { Url = "New url" }, ownItemInList.Id);
            Assert.True(result.IsSuccess);

            result = await _sut.ModifyItem(normalUser, new ModifyItemDto() { Url = "New url" }, ownItem2InList.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("Item with same url already in list", result.Error);
        }

        [Fact]
        public async Task ModifyItem_NoNameOrUrl_ItemNotModified() {
            SetupDb();

            var result = await _sut.ModifyItem(normalUser, new ModifyItemDto() { Name = null, Url = null }, ownItemInList.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("Item must have a name or url", result.Error);
        }

        [Fact]
        public async Task ModifyItem_UserModifyAmountOrdered_ItemNotModified() {
            SetupDb();

            var newAmountOrdered = ownItemInList.AmountOrdered + 1;
            var result = await _sut.ModifyItem(normalUser, new ModifyItemDto() { Name = ownItemInList.Name, AmountOrdered = newAmountOrdered }, ownItemInList.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("You can't modify amount ordered", result.Error);
        }

        [Fact]
        public async Task ModifyItem_UserModifyIsChecked_ItemNotModified() {
            SetupDb();

            var newIsChecked = !ownItemInList.IsChecked;
            var result = await _sut.ModifyItem(normalUser, new ModifyItemDto() { Name = ownItemInList.Name, IsChecked = newIsChecked }, ownItemInList.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("You can't modify isChecked", result.Error);
        }

        [Fact]
        public async Task ModifyItem_AdminModifyAmountOrdered_ItemModified() {
            SetupDb();

            var newAmountOrdered = ownItemInList.AmountOrdered + 1;
            var result = await _sut.ModifyItem(adminUser, new ModifyItemDto() { Name = ownItemInList.Name, AmountOrdered = newAmountOrdered }, ownItemInList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal(newAmountOrdered, item.AmountOrdered);
        }

        [Fact]
        public async Task ModifyItem_AdminModifyIsChecked_ItemModified() {
            SetupDb();

            var newIsChecked = !ownItemInList.IsChecked;
            var result = await _sut.ModifyItem(adminUser, new ModifyItemDto() { Name = ownItemInList.Name, IsChecked = newIsChecked }, ownItemInList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal(newIsChecked, item.IsChecked);
        }
        #endregion

        #region UpdateLikeStatus Tests
        [Fact]
        public async Task UpdateLikeStatus_NotLikedListItem_ItemIsLiked() {
            SetupDb();

            var result = await _sut.UpdateLikeStatus(normalUser, ownItemInList.Id, false);
            Assert.True(result.IsSuccess);

            var item = _context.Items.Include(i => i.UsersWhoLiked).FirstOrDefault(i => i.Id == ownItemInList.Id);
            Assert.NotNull(item);
            Assert.Contains(normalUser, item.UsersWhoLiked);
        }

        [Fact]
        public async Task UpdateLikeStatus_LikedListItem_ItemIsUnliked() {
            SetupDb();

            var result = await _sut.UpdateLikeStatus(normalUser, ownItemInList.Id, false);
            Assert.True(result.IsSuccess);

            result = await _sut.UpdateLikeStatus(normalUser, ownItemInList.Id, true);
            Assert.True(result.IsSuccess);

            var item = _context.Items.Include(i => i.UsersWhoLiked).FirstOrDefault(i => i.Id == ownItemInList.Id);
            Assert.NotNull(item);
            Assert.DoesNotContain(normalUser, item.UsersWhoLiked);
        }

        [Fact]
        public async Task UpdateLikeStatus_InvalidItem_Error() {
            SetupDb();

            var result = await _sut.UpdateLikeStatus(normalUser, itemInOrderedList.Id, false);
            Assert.False(result.IsSuccess);
            Assert.Equal("You can only (un)like active list's items", result.Error);
        }
        #endregion

        private void SetupDb() {
            _context.RemoveRange(_context.ShoppingLists.ToList());
            _context.RemoveRange(_context.Items.ToList());
            _context.RemoveRange(_context.Offices.ToList());
            _context.RemoveRange(_context.Users.ToList());
            _context.RemoveRange(_context.UserRoles.ToList());
            _context.SaveChanges();

            _context.UserRoles.AddRange(normalRole, adminRole);
            _context.SaveChanges();
            _context.Users.AddRange(normalUser, adminUser);
            _context.SaveChanges();
            _context.Offices.AddRange(testOffice, listOffice);
            _context.SaveChanges();
            _context.ShoppingLists.AddRange(normalList, dueDatePassedList, notStartedList, orderedList);
            _context.SaveChanges();
            _context.Items.AddRange(ownItemInList, ownItem2InList, othersItemInList, itemInDueDatePassedList, itemInOrderedList);
            _context.SaveChanges();
        }
        
    }
}
