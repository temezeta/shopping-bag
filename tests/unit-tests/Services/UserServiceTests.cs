using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using shopping_bag.Config;
using shopping_bag.DTOs.User;
using shopping_bag.Services;
using shopping_bag.Utility;

namespace shopping_bag_unit_tests.Services
{
    public class UserServiceTests : BaseServiceTest
    {

        private readonly AppDbContext _context;
        private readonly UserService _sut;
        private readonly Mock<IEmailService> _emailServiceMock = new Mock<IEmailService>();

        public UserServiceTests() : base()
        {
            _context = GetDatabase();
            _sut = new UserService(_context, _emailServiceMock.Object);
        }

        #region GetUserByEmail Tests
        [Fact]
        public async Task GetUserByEmail_ValidEmail_UserReturned()
        {
            var response = await _sut.GetUserByEmail("regular@huld.io");
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task GetUserByEmail_InvalidEmail_UserNotReturned()
        {
            // Removed account is still account in the database
            var response = await _sut.GetUserByEmail("admin2@huld.io");
            Assert.True(response.IsSuccess);
        }
        #endregion

        #region GetUserById Tests
        [Fact]
        public async Task GetUserById_ValidId_UserReturned()
        {
            var response = await _sut.GetUserById(1);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task GetUserById_InvalidId_UserNotReturned()
        {
            var response = await _sut.GetUserById(14);
            Assert.False(response.IsSuccess);
            Assert.Equal("User not found", response.Error);
        }
        #endregion

        #region GetUsers Tests
        [Fact]
        public async Task GetUsers_ValidUsersAdded_UsersListReturned()
        {
            var response = await _sut.GetUsers();
            Assert.True(response.IsSuccess);
            // One is removed thus not shown
            Assert.Equal(2, response.Data.Count());
        }

        #endregion

        #region RemoveUser Tests
        [Fact]
        public async Task RemoveUser_ValidUser_UserRemovedOnlyOnce()
        {
            // User is trying to remove themselves
            var response = await _sut.RemoveUser(NormalUser, 1);
            Assert.True(response.IsSuccess);

            // Try remove again
            response = await _sut.RemoveUser(NormalUser, 1);
            Assert.False(response.IsSuccess);
            Assert.Equal("User not found", response.Error);
        }

        [Fact]
        public async Task RemoveUser_UserHasNoPermission_UserNotRemoved()
        {
            var response = await _sut.RemoveUser(NormalUser, 2);
            Assert.False(response.IsSuccess);
            Assert.Equal("You can only remove your own account", response.Error);
        }

        #endregion

        #region Change Password
        [Fact]
        public async Task ChangeUserPassword_UserNotFound_ReturnsError()
        {
            var res = await _sut.ChangeUserPassword(4, new ChangePasswordDto
            {
                CurrentPassword = "string1A?",
                NewPassword = "string1B?",
                RepeatNewPassword = "string1B?"
            });

            Assert.NotNull(res);
            Assert.False(res.IsSuccess);
            Assert.Equal("User not found", res.Error);
        }

        [Fact]
        public async Task ChangeUserPassword_PasswordDoesNotMatch_ReturnsError()
        {
            var changeDto = new ChangePasswordDto
            {
                CurrentPassword = "string1A?",
                NewPassword = "string1B?",
                RepeatNewPassword = "string1B?"
            };
            AuthHelper.CreatePasswordHash("string1C?", out byte[] passwordHash, out byte[] passwordSalt);
            var user =  _context.Users.Find(1L);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _context.SaveChanges();

            var res = await _sut.ChangeUserPassword(1, changeDto);

            Assert.NotNull(res);
            Assert.False(res.IsSuccess);
            Assert.Equal("Invalid password", res.Error);
        }

        [Fact]
        public async Task ChangeUserPassword_PasswordChanged_ReturnsUser()
        {
            var changeDto = new ChangePasswordDto
            {
                CurrentPassword = "string1A?",
                NewPassword = "string1B?",
                RepeatNewPassword = "string1B?"
            };
            AuthHelper.CreatePasswordHash("string1A?", out byte[] passwordHash, out byte[] passwordSalt);
            var user = _context.Users.Find(1L);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _context.SaveChanges();

            var res = await _sut.ChangeUserPassword(1, changeDto);

            Assert.NotNull(res);
            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.NotEqual(passwordHash, res.Data.PasswordHash);
            Assert.NotEqual(passwordSalt, res.Data.PasswordSalt);
        }
        #endregion
    }
}
