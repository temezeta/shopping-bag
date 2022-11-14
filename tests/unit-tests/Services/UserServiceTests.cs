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

        public UserServiceTests() : base()
        {
            _context = GetDatabase();
            _sut = new UserService(_context);
        }

        #region Change Password
        [Fact]
        public async Task ChangeUserPassword_UserNotFound_ReturnsError()
        {
            var res = await _sut.ChangeUserPassword(3, new ChangePasswordDto
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
