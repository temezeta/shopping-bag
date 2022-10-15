using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using shopping_bag.Controllers;
using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Models;
using shopping_bag.Models.User;
using shopping_bag.Services;
using shopping_bag.Utility;

namespace shopping_bag_unit_tests.Controllers {
    public class ItemControllerTests {

        private readonly ItemController _sut;
        private readonly Mock<IUserService> _userService;
        private readonly Mock<IShoppingListService> _shoppingListService;

        public ItemControllerTests() {
            _userService = new Mock<IUserService>();
            _userService.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new ServiceResponse<User>(new User()));
            _shoppingListService = new Mock<IShoppingListService>();
            var profile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            var mapper = new Mapper(configuration);
            _sut = new ItemController(_userService.Object, _shoppingListService.Object, mapper) {
                ControllerContext = UnitTestHelper.GetLoggedInControllerContext()
            };
        }

        #region AddItemToShoppingList Tests
        [Fact]
        public async Task AddItemToShoppingList_ServiceResponseOk_ReturnsOk() {
            var serviceResponse = new ServiceResponse<Item>(new Item());
            _shoppingListService.Setup(x => x.AddItemToShoppingList(It.IsAny<AddItemDto>())).ReturnsAsync(serviceResponse);
            var result= await _sut.AddItemToShoppingList(new AddItemDto(), 0);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.IsType<ItemDto>(okResult.Value);
        }

        [Fact]
        public async Task AddItemToShoppingList_ServiceResponseError_ReturnsBadRequest() {
            var serviceResponse = new ServiceResponse<Item>(error: "Error");
            _shoppingListService.Setup(x => x.AddItemToShoppingList(It.IsAny<AddItemDto>())).ReturnsAsync(serviceResponse);
            var result = await _sut.AddItemToShoppingList(new AddItemDto(), 0);
            var brResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(brResult);
            Assert.NotNull(brResult.Value);
            Assert.Equal("Error", brResult.Value);
        }
        #endregion

        #region RemoveItemFromShoppingList Tests
        [Fact]
        public async Task RemoveItemFromShoppingList_ServiceResponseOk_ReturnsOk() {
            var serviceResponse = new ServiceResponse<bool>(true);
            _shoppingListService.Setup(x => x.RemoveItemFromShoppingList(It.IsAny<User>(), It.IsAny<long>())).ReturnsAsync(serviceResponse);
            var result = await _sut.RemoveItemFromShoppingList(0);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task RemoveItemFromShoppingList_ServiceResponseError_ReturnsBadRequest() {
            var serviceResponse = new ServiceResponse<bool>(error: "Error");
            _shoppingListService.Setup(x => x.RemoveItemFromShoppingList(It.IsAny<User>(), It.IsAny<long>())).ReturnsAsync(serviceResponse);
            var result = await _sut.RemoveItemFromShoppingList(0);
            var brResult = result as BadRequestObjectResult;
            Assert.NotNull(brResult);
            Assert.NotNull(brResult.Value);
            Assert.Equal("Error", brResult.Value);
        }
        #endregion

        #region ModifyItem Tests
        [Fact]
        public async Task ModifyItem_ServiceResponseOk_ReturnsOk() {
            var serviceResponse = new ServiceResponse<Item>(new Item());
            _shoppingListService.Setup(x => x.ModifyItem(It.IsAny<User>(), It.IsAny<ModifyItemDto>(), It.IsAny<long>())).ReturnsAsync(serviceResponse);
            var result = await _sut.ModifyItem(new ModifyItemDto(), 0);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.IsType<ItemDto>(okResult.Value);
        }

        [Fact]
        public async Task ModifyItem_ServiceResponseError_ReturnsBadRequest() {
            var serviceResponse = new ServiceResponse<Item>(error: "Error");
            _shoppingListService.Setup(x => x.ModifyItem(It.IsAny<User>(), It.IsAny<ModifyItemDto>(), It.IsAny<long>())).ReturnsAsync(serviceResponse);
            var result = await _sut.ModifyItem(new ModifyItemDto(), 0);
            var brResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(brResult);
            Assert.NotNull(brResult.Value);
            Assert.Equal("Error", brResult.Value);
        }
        #endregion
    }
}
