using AutoMapper;
using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Models;
using shopping_bag.Services;
using shopping_bag.Utility;

namespace shopping_bag_unit_tests.Services {
    public class ShoppingListServiceTests {

        private readonly AppDbContext _context;
        private readonly ShoppingListService _sut;
        public ShoppingListServiceTests() {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();

            var profile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            var mapper = new Mapper(configuration);
            _sut = new ShoppingListService(_context, mapper);
        }

        #region AddItemToShoppingList Tests
        [Fact]
        public async Task AddItemToShoppingList_ValidItem_ItemAdded() {
            SetupDb();

            // Ensure service result success.
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = 2, Name = "Test item" });
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data.ShoppingListId);

            // Ensure item added to list
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Id == result.Data.ShoppingListId);
            Assert.NotNull(list);
            Assert.NotNull(list.Items);
            Assert.Single(list.Items);
            Assert.Equal("Test item", list.Items.First().Name);
        }

        [Fact]
        public async Task AddItemToShoppingList_InvalidListId_ReturnsError() {
            SetupDb();

            // Ensure service result error
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = -1, Name = "Test item" });
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid shoppingListId", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any());
            Assert.Null(list);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(null, "")]
        public async Task AddItemToShoppingList_MissingNameOrUrl_ReturnsError(string name, string url) {
            SetupDb();

            // Ensure service result error
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = 1, Name = name, Url = url });
            Assert.False(result.IsSuccess);
            Assert.Equal("Item url or name must be given", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any());
            Assert.Null(list);
        }

        [Fact]
        public async Task AddItemToShoppingList_Ordered_ReturnsError() {
            SetupDb();

            _context.ShoppingLists.Add(new ShoppingList() { Id = 3, Name = "Test list 3", Ordered = true });
            _context.SaveChanges();

            // Ensure service result error
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = 3, Name = "Test item" });
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list already ordered", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any());
            Assert.Null(list);
        }

        [Fact]
        public async Task AddItemToShoppingList_DueDatePassed_ReturnsError() {
            SetupDb();

            _context.ShoppingLists.Add(new ShoppingList() { Id = 3, Name = "Test list 3", DueDate = DateTime.Now.AddMinutes(-10) });
            _context.SaveChanges();

            // Ensure service result error
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = 3, Name = "Test item" });
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list due date passed", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any());
            Assert.Null(list);
        }

        [Fact]
        public async Task AddItemToShoppingList_ListNotOpenYet_ReturnsError() {
            SetupDb();

            _context.ShoppingLists.Add(new ShoppingList() { Id = 3, Name = "Test list 3", StartDate = DateTime.Now.AddMinutes(10) });
            _context.SaveChanges();

            // Ensure service result error
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = 3, Name = "Test item" });
            Assert.False(result.IsSuccess);
            Assert.Equal("Shopping list not open yet", result.Error);

            // Ensure no items added to lists.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Any());
            Assert.Null(list);
        }

        [Fact]
        public async Task AddItemToShoppingList_DuplicateName_ReturnsError() {
            SetupDb();

            // Ensure service result ok
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = 1, Name = "Test item" });
            Assert.True(result.IsSuccess);

            // Ensure service result error
            result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = 1, Name = "Test item" });
            Assert.False(result.IsSuccess);
            Assert.Equal("Item with same name already in list", result.Error);

            // Ensure only first item added to list.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Count == 1);
            Assert.NotNull(list);
        }

        [Fact]
        public async Task AddItemToShoppingList_DuplicateUrl_ReturnsError() {
            SetupDb();

            // Ensure service result ok
            var result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = 1, Url = "http://example.com" });
            Assert.True(result.IsSuccess);

            // Ensure service result error
            result = await _sut.AddItemToShoppingList(new AddItemDto() { ShoppingListId = 1, Url = "http://example.com" });
            Assert.False(result.IsSuccess);
            Assert.Equal("Item with same url already in list", result.Error);

            // Ensure only first item added to list.
            var list = _context.ShoppingLists.Include(s => s.Items).FirstOrDefault(s => s.Items.Count == 1);
            Assert.NotNull(list);
        }

        private void SetupDb() {
            _context.RemoveRange(_context.ShoppingLists.ToList());
            _context.SaveChanges();
            Assert.Empty(_context.ShoppingLists.ToList());

            _context.ShoppingLists.Add(new ShoppingList() { Id = 1, Name = "Test list" });
            _context.ShoppingLists.Add(new ShoppingList() { Id = 2, Name = "Test list 2" });
            _context.SaveChanges();
            Assert.Equal(2, _context.ShoppingLists.ToList().Count);
        }
        #endregion
    }
}
