using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using shopping_bag.Config;
using shopping_bag.DTOs.User;
using shopping_bag.Models;
using shopping_bag.Models.Email;
using shopping_bag.Models.User;
using shopping_bag.Services;
using shopping_bag.Utility;
using shopping_bag_unit_tests.Services;

namespace shopping_bag_unit_tests
{
    public class AuthServiceTests : BaseServiceTest
    {
        private AuthService _authServiceMock;
        private readonly AppDbContext _appDbContext;
        private readonly Mock<IUserService> _iUserServiceMock = new Mock<IUserService>();
        private readonly Mock<IEmailService> _emailServiceMock = new Mock<IEmailService>();

        public AuthServiceTests() : base()
        {
            _appDbContext = GetDatabase();
            _authServiceMock = new AuthService(_appDbContext, _iUserServiceMock.Object, _emailServiceMock.Object);
        }

        #region Login/Logout Tests
        [Fact]
        public async void Login_LoginSuccessful_ReturnToken()
        {
            var email = "testemail@test.com";
            var password = "testpassword";
            byte[] passwordHash;
            byte[] passwordSalt;

            AuthHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User() { Email = email, PasswordHash = passwordHash, PasswordSalt = passwordSalt, VerifiedAt = DateTime.Now };
            var userResponse = new ServiceResponse<User>(data: user);
            _iUserServiceMock.Setup(x => x.GetUserByEmail(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(userResponse);

            // Act
            var loginDto = new LoginDto() { Email = email, Password = password };
            var loginResponse = await _authServiceMock.Login(loginDto);

            // Assert
            Assert.True(loginResponse.IsSuccess, "Login failed");
        }

        [Fact]
        public async void Logout_LogoutSuccessful_ReturnBoolean()
        {
            // Arrange
            var email = "testemail@test.com";
            var password = "testpassword";
            byte[] passwordHash;
            byte[] passwordSalt;

            AuthHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            var user = new User() { Email = email, FirstName = "testName", LastName = "testLastName", 
                PasswordHash = passwordHash, PasswordSalt = passwordSalt, Id = 1 };

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
     
            var dataBaseContext = new AppDbContext(options);
            dataBaseContext.Database.EnsureCreated();
            dataBaseContext.Users.Add(user);
            await dataBaseContext.SaveChangesAsync();
            _authServiceMock = new AuthService(dataBaseContext, _iUserServiceMock.Object, _emailServiceMock.Object);

            // Act
            var logOutResponse = await _authServiceMock.Logout(user);

            // Assert
            Assert.True(logOutResponse.IsSuccess, "Logout failed");
        }
        #endregion

        #region ResendVerificationEmail Tests

        [Fact]
        public async Task ResendVerificationEmail_ValidEmailAndUser_EmailResent()
        {
            _emailServiceMock.Setup(x => x.SendEmail(It.IsAny<Email>())).Returns(new ServiceResponse<bool>(true));
            var getUser = new ServiceResponse<User>(data: UnverifiedUser);
            _iUserServiceMock.Setup(x => x.GetUserByEmail(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(getUser);
            var hexToken = AuthHelper.CreateHexToken();
            var bodyText = "Something sensible";

            var response = await _authServiceMock.ResendVerificationEmail(UnverifiedUser.Email, hexToken, bodyText);
            Assert.True(response.IsSuccess);
            Assert.NotNull(UnverifiedUser.VerificationToken);
        }

        [Fact]
        public async Task ResendVerificationEmail_ValidEmailUserAlreadyVerified_EmailNotSent()
        {
            _emailServiceMock.Setup(x => x.SendEmail(It.IsAny<Email>())).Returns(new ServiceResponse<bool>(false));
            var getUser = new ServiceResponse<User>(data: NormalUser);
            _iUserServiceMock.Setup(x => x.GetUserByEmail(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(getUser);
            var hexToken = AuthHelper.CreateHexToken();
            var bodyText = "Something sensible";

            var response = await _authServiceMock.ResendVerificationEmail(UnverifiedUser.Email, hexToken, bodyText);
            Assert.False(response.IsSuccess);
            Assert.Equal("User is already verified", response.Error);
        }
        #endregion
    }
}
