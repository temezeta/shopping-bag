﻿using Moq;
using shopping_bag.Controllers;
using shopping_bag.Models.User;
using shopping_bag.Models;
using shopping_bag.Services;
using shopping_bag.DTOs.ShoppingList;
using Microsoft.AspNetCore.Mvc;

namespace shopping_bag_unit_tests.Controllers
{
    public class ShoppingListControllerTests
    {
        private readonly ShoppingListController _sut;
        private readonly Mock<IUserService> _userService = new Mock<IUserService>();
        private readonly Mock<IShoppingListService> _shoppingListService = new Mock<IShoppingListService>();
        public ShoppingListControllerTests()
        {
            _userService.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new ServiceResponse<User>(new User()));
            _sut = new ShoppingListController(_userService.Object, _shoppingListService.Object, UnitTestHelper.GetMapper())
            {
                ControllerContext = UnitTestHelper.GetLoggedInControllerContext()
            };
        }

        [Fact]
        public async Task SetOrderedAmount_SetFailed_ReturnsBadRequest()
        {
            _shoppingListService.Setup(it => it.SetOrderedAmount(It.IsAny<long>(), It.IsAny<OrderedAmountDto>()))
                .ReturnsAsync(new ServiceResponse<ShoppingList>(error: "Error"));

            var response = await _sut.SetOrderedAmount(1, new OrderedAmountDto());

            Assert.NotNull(response);
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task SetOrderedAmount_SetSuccess_ReturnsBadRequest()
        {
            _shoppingListService.Setup(it => it.SetOrderedAmount(It.IsAny<long>(), It.IsAny<OrderedAmountDto>()))
                .ReturnsAsync(new ServiceResponse<ShoppingList>(new ShoppingList()));

            var response = await _sut.SetOrderedAmount(1, new OrderedAmountDto());

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);
        }
    }
}
