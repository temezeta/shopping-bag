using Moq;
using shopping_bag.Controllers;
using shopping_bag.DTOs.User;
using shopping_bag.Models;
using shopping_bag.Services;
using BadRequestResult = Microsoft.AspNetCore.Mvc.BadRequestResult;

namespace shopping_bag_unit_tests
{
    public class AuthControllerTests
    {
        private AuthController _authControllerMock;
        private readonly Mock<IAuthService> _authServiceMock = new Mock<IAuthService>();
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly Mock<IEmailService> _emailServiceMock = new Mock<IEmailService>();

        public AuthControllerTests()
        {
            _authControllerMock = new AuthController(_userServiceMock.Object, _authServiceMock.Object, _emailServiceMock.Object);
        }

        [Fact]
        public async void Register_EmailAlreadyExists_ReturnsBadRequestResult()
        {
            var authServiceResponse = new ServiceResponse<string>(error: "User with email already exists");
            _authServiceMock.Setup(x => x.Register(It.IsAny<RegisterDto>())).ReturnsAsync(authServiceResponse);

            var registerResponse = await _authControllerMock.Register(It.IsAny<RegisterDto>());

            Assert.IsType<BadRequestResult>(registerResponse);
        }
    }
}