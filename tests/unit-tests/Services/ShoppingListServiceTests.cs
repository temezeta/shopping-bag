using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Expressions;
using Moq;
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
            _sut = new ShoppingListService(_context, UnitTestHelper.GetMapper(), new Mock<IReminderService>().Object);
        }

        #region AddShoppingList Tests
        [Fact]
        public async Task AddShoppingList_ListWithOnlyNameAndOffice_ShoppingListAdded()
        {
            var result = await _sut.AddShoppingList(new AddShoppingListDto() { Name = "Office supplies", OfficeId = 1, UserId = 3});
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task AddShoppingList_FutureDueDate_Ok() {
            var result = await _sut.AddShoppingList(new AddShoppingListDto() { Name = "Office supplies", OfficeId = TestOffice.Id, UserId = NormalUser.Id, DueDate = DateTime.Now.AddDays(1) });
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task AddShoppingList_PastDueDate_Error() {
            var result = await _sut.AddShoppingList(new AddShoppingListDto() { Name = "Office supplies", OfficeId = TestOffice.Id, UserId = NormalUser.Id, DueDate = DateTime.Now.AddDays(-1) });
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list due date passed", result.Error);
        }

        [Fact]
        public async Task AddShoppingList_FutureExpectedDeliveryDate_Ok() {
            var result = await _sut.AddShoppingList(new AddShoppingListDto() { Name = "Office supplies", OfficeId = TestOffice.Id, UserId = NormalUser.Id, ExpectedDeliveryDate = DateTime.Now.AddDays(1) });
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task AddShoppingList_PastExpectedDeliveryDate_Error() {
            var result = await _sut.AddShoppingList(new AddShoppingListDto() { Name = "Office supplies", OfficeId = TestOffice.Id, UserId = NormalUser.Id, ExpectedDeliveryDate = DateTime.Now.AddDays(-1) });
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list expected delivery date passed", result.Error);
        }
        #endregion

        #region GetShoppingListById Tests
        [Fact]
        public async Task GetShoppingListById_ValidId_ShoppingListReturned() {
            var response = await _sut.GetShoppingListById(NormalList.Id);
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

        [Fact]
        public async Task OrderShoppingList_ValidShoppingList_OrderDateUpdated() {
            var response = await _sut.OrderShoppingList(1);
            Assert.True(response.IsSuccess);
            var now = DateTime.Now;
            var list = await _context.ShoppingLists.FirstOrDefaultAsync(l => l.Id == 1);
            Assert.NotNull(list);
            Assert.True(list.Ordered);
            Assert.True(list.OrderedDate.HasValue);
            Assert.True(now.Subtract(list.OrderedDate.Value) < new TimeSpan(0, 0, 10)); // Date within 10s, test shouldn't take longer.
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
            var remove = await _sut.RemoveShoppingList(NormalList.Id);
            Assert.True(remove.IsSuccess);

            var removeAgain = await _sut.RemoveShoppingList(NormalList.Id);
            Assert.False(removeAgain.IsSuccess);
            Assert.Equal("Invalid shoppingListId", removeAgain.Error);
        }

        #endregion


        #region AddItemToShoppingList Tests
        [Fact]
        public async Task AddItem_ValidItem_ItemAdded() {
            // Ensure service result success.
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = NormalList.Id, Name = "Test item" });
            Assert.True(result.IsSuccess);
            Assert.Equal(NormalList.Id, result.Data.ShoppingListId);

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
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = NormalList.Id, Name = name, Url = url });
            Assert.False(result.IsSuccess);
            Assert.Equal("Item url or name must be given", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any(i => i.Name == name && i.Url == url));
            Assert.Null(list);
        }

        [Fact]
        public async Task AddItem_OrderedList_ReturnsError() {
            // Ensure service result error
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = OrderedList.Id, Name = "Test item" });
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list already ordered", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any(i => i.Name == "Test item"));
            Assert.Null(list);
        }

        [Fact]
        public async Task AddItem_DueDatePassedList_ReturnsError() {
            // Ensure service result error
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = DueDatePassedList.Id, Name = "Test item" });
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list due date passed", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any(i => i.Name == "Test item"));
            Assert.Null(list);
        }

        [Fact]
        public async Task AddItem_DuplicateUrl_ReturnsError() {
            // Ensure service result ok
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = NormalList.Id, Url = "http://example.com" });
            Assert.True(result.IsSuccess);

            // Ensure service result error
            result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = NormalList.Id, Url = "http://example.com" });
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
            var result = await _sut.RemoveItemFromShoppingList(NormalUser, OwnItemInList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == OwnItemInList.Id);
            Assert.Null(item);
        }

        [Fact]
        public async Task RemoveItem_UserValidListNotOwnItem_ItemRemoved() {
            var result = await _sut.RemoveItemFromShoppingList(NormalUser, OthersItemInList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == OthersItemInList.Id);
            Assert.Null(item);
        }

        [Fact]
        public async Task RemoveItem_AdminValidListNotOwnItem_ItemRemoved() {
            var result = await _sut.RemoveItemFromShoppingList(AdminUser, OthersItemInList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == OthersItemInList.Id);
            Assert.Null(item);
        }

        [Fact]
        public async Task RemoveItem_UserDueDatePassedList_ItemNotRemoved() {
            var result = await _sut.RemoveItemFromShoppingList(NormalUser, ItemInDueDatePassedList.Id);
            Assert.False(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == ItemInDueDatePassedList.Id);
            Assert.NotNull(item);
        }

        [Fact]
        public async Task RemoveItem_AdminDueDatePassedList_ItemRemoved() {
            var result = await _sut.RemoveItemFromShoppingList(AdminUser, ItemInDueDatePassedList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == ItemInDueDatePassedList.Id);
            Assert.Null(item);
        }
        #endregion

        #region ModifyItem Tests
        [Fact]
        public async Task ModifyItem_UserValidItem_ItemModified() {
            var result = await _sut.ModifyItem(NormalUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, OwnItemInList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal("Test item", item.Name);
            Assert.Equal("New url", item.Url);
        }
        [Fact]
        public async Task ModifyItem_AdminNotOwnItem_ItemModified() {
            var result = await _sut.ModifyItem(AdminUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, OthersItemInList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal("Test item", item.Name);
            Assert.Equal("New url", item.Url);
        }

        [Fact]
        public async Task ModifyItem_UserNotOwnItem_ItemModified() {
            var result = await _sut.ModifyItem(NormalUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, OthersItemInList.Id);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ModifyItem_InvalidItemId_ItemNotModified() {
            var result = await _sut.ModifyItem(NormalUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, -1);
            Assert.False(result.IsSuccess);
            Assert.Equal("Item doesn't exist.", result.Error);
        }

        [Fact]
        public async Task ModifyItem_UserOrderedList_ItemNotModified() {
            var result = await _sut.ModifyItem(NormalUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, ItemInOrderedList.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list already ordered", result.Error);
        }

        [Fact]
        public async Task ModifyItem_AdminOrderedList_ItemModified() {
            var result = await _sut.ModifyItem(AdminUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, ItemInOrderedList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal("Test item", item.Name);
            Assert.Equal("New url", item.Url);
        }

        [Fact]
        public async Task ModifyItem_UserDueDatePassedList_ItemNotModified() {
            var result = await _sut.ModifyItem(NormalUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, ItemInDueDatePassedList.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list due date passed", result.Error);
        }

        [Fact]
        public async Task ModifyItem_AdminDueDatePassedList_ItemModified() {
            var result = await _sut.ModifyItem(AdminUser, new ModifyItemDto() { Name = "Test item", Url = "New url" }, ItemInDueDatePassedList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal("Test item", item.Name);
            Assert.Equal("New url", item.Url);
        }

        [Fact]
        public async Task ModifyItem_DuplicateUrl_ItemNotModified() {
            var result = await _sut.ModifyItem(NormalUser, new ModifyItemDto() { Url = "New url" }, OwnItemInList.Id);
            Assert.True(result.IsSuccess);

            result = await _sut.ModifyItem(NormalUser, new ModifyItemDto() { Url = "New url" }, OwnItem2InList.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("Item with same url already in list", result.Error);
        }

        [Fact]
        public async Task ModifyItem_NoNameOrUrl_ItemNotModified() {
            var result = await _sut.ModifyItem(NormalUser, new ModifyItemDto() { Name = null, Url = null }, OwnItemInList.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("Item must have a name or url", result.Error);
        }

        [Fact]
        public async Task ModifyItem_UserModifyAmountOrdered_ItemNotModified() {
            var newAmountOrdered = OwnItemInList.AmountOrdered + 1;
            var result = await _sut.ModifyItem(NormalUser, new ModifyItemDto() { Name = OwnItemInList.Name, AmountOrdered = newAmountOrdered }, OwnItemInList.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("You can't modify amount ordered", result.Error);
        }

        [Fact]
        public async Task ModifyItem_UserModifyIsChecked_ItemNotModified() {
            var newIsChecked = !OwnItemInList.IsChecked;
            var result = await _sut.ModifyItem(NormalUser, new ModifyItemDto() { Name = OwnItemInList.Name, IsChecked = newIsChecked }, OwnItemInList.Id);
            Assert.False(result.IsSuccess);
            Assert.Equal("You can't modify isChecked", result.Error);
        }

        [Fact]
        public async Task ModifyItem_AdminModifyAmountOrdered_ItemModified() {
            var newAmountOrdered = OwnItemInList.AmountOrdered + 1;
            var result = await _sut.ModifyItem(AdminUser, new ModifyItemDto() { Name = OwnItemInList.Name, AmountOrdered = newAmountOrdered }, OwnItemInList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal(newAmountOrdered, item.AmountOrdered);
        }

        [Fact]
        public async Task ModifyItem_AdminModifyIsChecked_ItemModified() {
            var newIsChecked = !OwnItemInList.IsChecked;
            var result = await _sut.ModifyItem(AdminUser, new ModifyItemDto() { Name = OwnItemInList.Name, IsChecked = newIsChecked }, OwnItemInList.Id);
            Assert.True(result.IsSuccess);

            var item = _context.Items.FirstOrDefault(i => i.Id == result.Data.Id);
            Assert.NotNull(item);
            Assert.Equal(newIsChecked, item.IsChecked);
        }
        #endregion

        #region UpdateLikeStatus Tests
        [Fact]
        public async Task UpdateLikeStatus_NotLikedListItem_ItemIsLiked() {
            var result = await _sut.UpdateLikeStatus(NormalUser, OwnItemInList.Id, false);
            Assert.True(result.IsSuccess);

            var item = _context.Items.Include(i => i.UsersWhoLiked).FirstOrDefault(i => i.Id == OwnItemInList.Id);
            Assert.NotNull(item);
            Assert.Contains(NormalUser, item.UsersWhoLiked);
        }

        [Fact]
        public async Task UpdateLikeStatus_LikedListItem_ItemIsUnliked() {
            var result = await _sut.UpdateLikeStatus(NormalUser, OwnItemInList.Id, false);
            Assert.True(result.IsSuccess);

            result = await _sut.UpdateLikeStatus(NormalUser, OwnItemInList.Id, true);
            Assert.True(result.IsSuccess);

            var item = _context.Items.Include(i => i.UsersWhoLiked).FirstOrDefault(i => i.Id == OwnItemInList.Id);
            Assert.NotNull(item);
            Assert.DoesNotContain(NormalUser, item.UsersWhoLiked);
        }

        [Fact]
        public async Task UpdateLikeStatus_InvalidItem_Error() {
            var result = await _sut.UpdateLikeStatus(NormalUser, ItemInOrderedList.Id, false);
            Assert.False(result.IsSuccess);
            Assert.Equal("You can only (un)like active list's items", result.Error);
        }
        #endregion

        #region SetOrderedAmount Tests
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
        #endregion Tests

        #region SetItemCheckedStatus Tests

        [Fact]
        public async Task SetItemCheckedStatus_ValidItemAndList_ReturnsShoppingList()
        {
            var checkedItem = new CheckedItemDto()
            {
                ItemId = 1,
                IsChecked = true
            };

            var response = await _sut.SetItemCheckedStatus(1, checkedItem);
            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
        }

        [Fact]
        public async Task SetItemCheckedStatus_InvalidList_ReturnsError()
        {
            var checkedItem = new CheckedItemDto()
            {
                ItemId = 1,
                IsChecked = true
            };

            var response = await _sut.SetItemCheckedStatus(2000, checkedItem);
            Assert.False(response.IsSuccess);
            Assert.Equal("Invalid shopping list", response.Error);
        }

        [Fact]
        public async Task SetItemCheckedStatus_ValidListInvalidItem_ReturnsError()
        {
            var checkedItem = new CheckedItemDto()
            {
                ItemId = 12982,
                IsChecked = true
            };

            var response = await _sut.SetItemCheckedStatus(1, checkedItem);
            Assert.False(response.IsSuccess);
            Assert.Equal("Item not on list", response.Error);
        }

        #endregion
    }
}
