using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Services;

namespace shopping_bag_unit_tests.Services {
    public class ShoppingListServiceTests : BaseServiceTest {

        private readonly AppDbContext _context;
        private readonly ShoppingListService _sut;
        public ShoppingListServiceTests() : base() 
        {
            _context = GetDatabase();
            _sut = new ShoppingListService(_context, UnitTestHelper.GetMapper());
        }

        #region AddShoppingList Tests
        [Fact]
        public async Task AddShoppingList_ListWithOnlyNameAndOffice_ShoppingListAdded()
        {
            var result = await _sut.AddShoppingList(new AddShoppingListDto() { Name = "Office supplies", OfficeId = 1, UserId = 3});
            Assert.True(result.IsSuccess);
        }
        #endregion

        #region GetShoppingListById Tests
        [Fact]
        public async Task GetShoppingListById_ValidId_ShoppingListReturned() {
            var response = await _sut.GetShoppingListById(ShoppingLists[0].Id);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task GetShoppingListById_InvalidId_ErrorReturned()
        {
            var response = await _sut.GetShoppingListById(827);
            Assert.False(response.IsSuccess);
            Assert.Equal("Invalid shoppingListId", response.Error);
        }
        #endregion

        #region GetShoppingListsByOffice Tests
        [Fact]
        public async Task GetShoppingListsByOffice_InvalidOfficeId_ErrorReturned()
        {
            var result = await _sut.GetShoppingListsByOffice(14);
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid officeId", result.Error);
        }

        [Fact]
        public async Task GetShoppingListsByOffice_ValidOfficeId_ListsReturned()
        {
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
            var list = await _sut.AddShoppingList(new AddShoppingListDto() { Name = "Office supplies", OfficeId = 1, UserId = 3 });
            Assert.True(list.IsSuccess);

            var result = await _sut.ModifyShoppingList(new ModifyShoppingListDto() { Name = "Office supplies 2"}, list.Data.Id);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ModifyShoppingList_ActiveListWithNameAlreadyExists_ListNotModified()
        {
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
            var remove = await _sut.RemoveShoppingList(ShoppingLists[0].Id);
            Assert.True(remove.IsSuccess);

            var removeAgain = await _sut.RemoveShoppingList(ShoppingLists[0].Id);
            Assert.False(removeAgain.IsSuccess);
            Assert.Equal("Invalid shoppingListId", removeAgain.Error);
        }

        #endregion


        #region AddItemToShoppingList Tests
        [Fact]
        public async Task AddItem_ValidItem_ItemAdded() {
            // Ensure service result success.
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = ShoppingLists[0].Id, Name = "Test item" });
            Assert.True(result.IsSuccess);
            Assert.Equal(ShoppingLists[0].Id, result.Data.ShoppingListId);

            // Ensure item added to list
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Id == result.Data.ShoppingListId);
            Assert.NotNull(list);
            Assert.NotNull(list.Items);
            Assert.Single(list.Items.Where(i => i.Id == result.Data.Id));
            Assert.Equal("Test item", list.Items.FirstOrDefault(i => i.Id == result.Data.Id)?.Name);
        }

        [Fact]
        public async Task AddItem_InvalidListId_ReturnsError() {
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
            // Ensure service result error
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = ShoppingLists[0].Id, Name = name, Url = url });
            Assert.False(result.IsSuccess);
            Assert.Equal("Item url or name must be given", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any(i => i.Name == name && i.Url == url));
            Assert.Null(list);
        }

        [Fact]
        public async Task AddItem_OrderedList_ReturnsError() {
            // Ensure service result error
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = ShoppingLists[3].Id, Name = "Test item" });
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list already ordered", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any(i => i.Name == "Test item"));
            Assert.Null(list);
        }

        [Fact]
        public async Task AddItem_DueDatePassedList_ReturnsError() {
            // Ensure service result error
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = ShoppingLists[1].Id, Name = "Test item" });
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list due date passed", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any(i => i.Name == "Test item"));
            Assert.Null(list);
        }

        [Fact]
        public async Task AddItem_DuplicateUrl_ReturnsError() {
            // Ensure service result ok
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = ShoppingLists[0].Id, Url = "http://example.com" });
            Assert.True(result.IsSuccess);

            // Ensure service result error
            result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = ShoppingLists[0].Id, Url = "http://example.com" });
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
            var result = await _sut.RemoveItemFromShoppingList(Users[0], Items[0].Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == Items[0].Id);
            Assert.Null(item);
        }

        [Fact]
        public async Task RemoveItem_UserValidListNotOwnItem_ItemNotRemoved() {
            var result = await _sut.RemoveItemFromShoppingList(Users[0], Items[2].Id);
            Assert.False(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == Items[2].Id);
            Assert.NotNull(item);
        }

        [Fact]
        public async Task RemoveItem_AdminValidListNotOwnItem_ItemRemoved() {
            var result = await _sut.RemoveItemFromShoppingList(Users[1], Items[2].Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == Items[2].Id);
            Assert.Null(item);
        }

        [Fact]
        public async Task RemoveItem_UserDueDatePassedList_ItemNotRemoved() {
            var result = await _sut.RemoveItemFromShoppingList(Users[0], Items[3].Id);
            Assert.False(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == Items[3].Id);
            Assert.NotNull(item);
        }

        [Fact]
        public async Task RemoveItem_AdminDueDatePassedList_ItemRemoved() {
            var result = await _sut.RemoveItemFromShoppingList(Users[1], Items[3].Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == Items[3].Id);
            Assert.Null(item);
        }
        #endregion

        #region ModifyItem Tests
        [Fact]
        public async Task ModifyItem_UserValidItem_ItemModified() {
            var result = await _sut.ModifyItem(Users[0], new ModifyItemDto() { Name = "Test item", Url = "New url" }, Items[0].Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal("Test item", item.Name);
            Assert.Equal("New url", item.Url);
        }
        [Fact]
        public async Task ModifyItem_AdminNotOwnItem_ItemModified() {
            var result = await _sut.ModifyItem(Users[1], new ModifyItemDto() { Name = "Test item", Url = "New url" }, Items[2].Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal("Test item", item.Name);
            Assert.Equal("New url", item.Url);
        }

        [Fact]
        public async Task ModifyItem_UserNotOwnItem_ItemNotModified() {
            var result = await _sut.ModifyItem(Users[0], new ModifyItemDto() { Name = "Test item", Url = "New url" }, Items[2].Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("You can only modify items you have added", result.Error);
        }

        [Fact]
        public async Task ModifyItem_InvalidItemId_ItemNotModified() {
            var result = await _sut.ModifyItem(Users[0], new ModifyItemDto() { Name = "Test item", Url = "New url" }, -1);
            Assert.False(result.IsSuccess);
            Assert.Equal("Item doesn't exist.", result.Error);
        }

        [Fact]
        public async Task ModifyItem_UserOrderedList_ItemNotModified() {
            var result = await _sut.ModifyItem(Users[0], new ModifyItemDto() { Name = "Test item", Url = "New url" }, Items[4].Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list already ordered", result.Error);
        }

        [Fact]
        public async Task ModifyItem_AdminOrderedList_ItemModified() {
            var result = await _sut.ModifyItem(Users[1], new ModifyItemDto() { Name = "Test item", Url = "New url" }, Items[4].Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal("Test item", item.Name);
            Assert.Equal("New url", item.Url);
        }

        [Fact]
        public async Task ModifyItem_UserDueDatePassedList_ItemNotModified() {
            var result = await _sut.ModifyItem(Users[0], new ModifyItemDto() { Name = "Test item", Url = "New url" }, Items[3].Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list due date passed", result.Error);
        }

        [Fact]
        public async Task ModifyItem_AdminDueDatePassedList_ItemModified() {
            var result = await _sut.ModifyItem(Users[1], new ModifyItemDto() { Name = "Test item", Url = "New url" }, Items[3].Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal("Test item", item.Name);
            Assert.Equal("New url", item.Url);
        }

        [Fact]
        public async Task ModifyItem_DuplicateUrl_ItemNotModified() {
            var result = await _sut.ModifyItem(Users[0], new ModifyItemDto() { Url = "New url" }, Items[0].Id);
            Assert.True(result.IsSuccess);

            result = await _sut.ModifyItem(Users[0], new ModifyItemDto() { Url = "New url" }, Items[1].Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("Item with same url already in list", result.Error);
        }

        [Fact]
        public async Task ModifyItem_NoNameOrUrl_ItemNotModified() {
            var result = await _sut.ModifyItem(Users[0], new ModifyItemDto() { Name = null, Url = null }, Items[0].Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("Item must have a name or url", result.Error);
        }

        [Fact]
        public async Task ModifyItem_UserModifyAmountOrdered_ItemNotModified() {
            var newAmountOrdered = Items[0].AmountOrdered + 1;
            var result = await _sut.ModifyItem(Users[0], new ModifyItemDto() { Name = Items[0].Name, AmountOrdered = newAmountOrdered }, Items[0].Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("You can't modify amount ordered", result.Error);
        }

        [Fact]
        public async Task ModifyItem_UserModifyIsChecked_ItemNotModified() {
            var newIsChecked = !Items[0].IsChecked;
            var result = await _sut.ModifyItem(Users[0], new ModifyItemDto() { Name = Items[0].Name, IsChecked = newIsChecked }, Items[0].Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("You can't modify isChecked", result.Error);
        }

        [Fact]
        public async Task ModifyItem_AdminModifyAmountOrdered_ItemModified() {
            var newAmountOrdered = Items[0].AmountOrdered + 1;
            var result = await _sut.ModifyItem(Users[1], new ModifyItemDto() { Name = Items[0].Name, AmountOrdered = newAmountOrdered }, Items[0].Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal(newAmountOrdered, item.AmountOrdered);
        }

        [Fact]
        public async Task ModifyItem_AdminModifyIsChecked_ItemModified() {
            var newIsChecked = !Items[0].IsChecked;
            var result = await _sut.ModifyItem(Users[1], new ModifyItemDto() { Name = Items[0].Name, IsChecked = newIsChecked }, Items[0].Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal(newIsChecked, item.IsChecked);
        }
        #endregion

        #region UpdateLikeStatus Tests
        [Fact]
        public async Task UpdateLikeStatus_NotLikedListItem_ItemIsLiked() {
            var result = await _sut.UpdateLikeStatus(Users[0], Items[0].Id, false);
            Assert.True(result.IsSuccess);

            var item = _context.Items.Include(i => i.UsersWhoLiked).FirstOrDefault(i => i.Id == Items[0].Id);
            Assert.NotNull(item);
            Assert.Contains(Users[0], item.UsersWhoLiked);
        }

        [Fact]
        public async Task UpdateLikeStatus_LikedListItem_ItemIsUnliked() {
            var result = await _sut.UpdateLikeStatus(Users[0], Items[0].Id, false);
            Assert.True(result.IsSuccess);

            result = await _sut.UpdateLikeStatus(Users[0], Items[0].Id, true);
            Assert.True(result.IsSuccess);

            var item = _context.Items.Include(i => i.UsersWhoLiked).FirstOrDefault(i => i.Id == Items[0].Id);
            Assert.NotNull(item);
            Assert.DoesNotContain(Users[0], item.UsersWhoLiked);
        }

        [Fact]
        public async Task UpdateLikeStatus_InvalidItem_Error() {
            var result = await _sut.UpdateLikeStatus(Users[0], Items[4].Id, false);
            Assert.False(result.IsSuccess);
            Assert.Equal("You can only (un)like active list's items", result.Error);
        }
        #endregion

        #region SetOrderedAmount
        [Fact]
        public async Task SetOrderedAmount_ListNotFound_ReturnsError()
        {
            var response = await _sut.SetOrderedAmount(100, new OrderedAmountDto
            {
                ItemId = 1,
                AmountOrdered = 2
            });

            Assert.False(response.IsSuccess);
            Assert.Equal("Invalid shopping list", response.Error);
        }

        [Fact]
        public async Task SetOrderedAmount_ListRemoved_ReturnsError()
        {
            var response = await _sut.SetOrderedAmount(5, new OrderedAmountDto
            {
                ItemId = 1,
                AmountOrdered = 2
            });

            Assert.False(response.IsSuccess);
            Assert.Equal("Invalid shopping list", response.Error);
        }

        [Fact]
        public async Task SetOrderedAmount_ItemNotOnList_ReturnsError()
        {
            var response = await _sut.SetOrderedAmount(1, new OrderedAmountDto
            {
                ItemId = 4,
                AmountOrdered = 2
            });

            Assert.False(response.IsSuccess);
            Assert.Equal("Item not on list", response.Error);
        }

        [Fact]
        public async Task SetOrderedAmount_AmountSet_ReturnsShoppingList()
        {
            var response = await _sut.SetOrderedAmount(1, new OrderedAmountDto
            {
                ItemId = 1,
                AmountOrdered = 2
            });

            Assert.True(response.IsSuccess);
            var list = response.Data;
            Assert.NotNull(list);
            Assert.Equal(1, list.Id);
            var item = list.Items.FirstOrDefault(it => it.Id == 1);
            Assert.NotNull(item);
            Assert.Equal(2, item.AmountOrdered);
        }
        #endregion
    }
}
