using Microsoft.Extensions.Configuration;
using Moq;
using shopping_bag.Config;
using shopping_bag.DTOs.User;
using shopping_bag.Models;
using shopping_bag.Models.User;
using shopping_bag.Services;
using shopping_bag.Utility;


namespace shopping_bag_unit_tests
{
    public class AuthServiceTests
    {
        private AuthService _authServiceMock;
        private readonly Mock<AppDbContext> _appDbContextMock = new Mock<AppDbContext>();
        private readonly Mock<IUserService> _iUserServiceMock = new Mock<IUserService>();
        public AuthServiceTests()
        {
            _authServiceMock = new AuthService(_appDbContextMock.Object, _iUserServiceMock.Object);
        }

        [Fact]
        public async void Login_LoginSuccessful_ReturnToken()
        {
            // Arrange
            var testConfiguration = new Dictionary<string, string>
            {
                {"Jwt:Token", "superlongssecretwritesomethinghere"},
            };

            var configuration = new ConfigurationBuilder().AddInMemoryCollection(testConfiguration).Build();
            StaticConfig.Setup(configuration);

            var email = "testemail@test.com";
            var password = "testpassword";
            byte[] passwordHash;
            byte[] passwordSalt;

            AuthHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User() { Email = email, PasswordHash = passwordHash, PasswordSalt = passwordSalt, VerifiedAt = DateTime.Now };
            var userResponse = new ServiceResponse<User>(data: user);
            _iUserServiceMock.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(userResponse);

            // Act
            var loginDto = new LoginDto() { Email = email, Password = password };
            var loginResponse = await _authServiceMock.Login(loginDto);

            // Assert
            Assert.True(loginResponse.IsSuccess, "Login failed");
        }
    }
}
