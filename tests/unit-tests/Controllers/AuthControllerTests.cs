using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using shopping_bag.Config;
using shopping_bag.Controllers;
using shopping_bag.DTOs.User;
using shopping_bag.Models;
using shopping_bag.Models.User;
using shopping_bag.Services;
using shopping_bag.Utility;
using System;
using BadRequestResult = Microsoft.AspNetCore.Mvc.BadRequestResult;

namespace shopping_bag_unit_tests
{
    public class AuthControllerTests
    {
        private AuthController _authControllerMock;
        private readonly Mock<IAuthService> _authServiceMock = new Mock<IAuthService>();
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();

        public AuthControllerTests()
        {
            _authControllerMock = new AuthController(_userServiceMock.Object, _authServiceMock.Object)
            {
                Url = UnitTestHelper.GetUrlHelper()
            };
        }

        [Fact]
        public async void Register_EmailAlreadyExists_ReturnsBadRequestResult()
        {
            UnitTestHelper.SetupStaticConfig();
            var authServiceResponse = new ServiceResponse<bool>(error: "User with email already exists");
            _authServiceMock.Setup(x => x.Register(It.IsAny<RegisterDto>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(authServiceResponse);

            var registerResponse = await _authControllerMock.Register(It.IsAny<RegisterDto>());

            Assert.IsType<BadRequestResult>(registerResponse);
        }

        [Fact]
        public async void Logout_LoggedInUser_ReturnsOk() {
            _authControllerMock.ControllerContext = UnitTestHelper.GetLoggedInControllerContext();

            var authServiceResponse = new ServiceResponse<bool>(data: true);
            _authServiceMock.Setup(x => x.Logout(It.IsAny<User>())).ReturnsAsync(authServiceResponse);
            var userServiceResponse = new ServiceResponse<User>(data: new User());
            _userServiceMock.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(userServiceResponse);

            var registerResponse = await _authControllerMock.Logout();

            Assert.IsType<OkObjectResult>(registerResponse);
        }
    }
}