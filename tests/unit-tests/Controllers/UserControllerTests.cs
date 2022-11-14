using Moq;
using shopping_bag.Controllers;
using shopping_bag.Models.User;
using shopping_bag.Models;
using shopping_bag.Services;
using shopping_bag.DTOs.User;
using Microsoft.AspNetCore.Mvc;

namespace shopping_bag_unit_tests.Controllers
{
    public class UserControllerTests
    {
        private readonly UserController _sut;
        private readonly Mock<IUserService> _userService = new Mock<IUserService>();

        public UserControllerTests()
        {
            _userService.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new ServiceResponse<User>(new User()));
            _sut = new UserController(_userService.Object, UnitTestHelper.GetMapper())
            {
                ControllerContext = UnitTestHelper.GetLoggedInControllerContext()
            };
        }

        #region Change Password
        [Fact]
        public async Task ChangePassword_UserNotFound_ReturnsBadRequest()
        {
            _userService.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new ServiceResponse<User>(error: "User not found"));

            var res = await _sut.ChangePassword(new ChangePasswordDto
            {
                CurrentPassword = "string1A?",
                NewPassword = "string1B?",
                RepeatNewPassword = "string1B?"
            });

            Assert.IsType<BadRequestResult>(res);
        }

        [Fact]
        public async Task ChangePassword_ChangeFailed_ReturnsBadRequest()
        {
            _userService.Setup(it => it.ChangeUserPassword(It.IsAny<long>(), It.IsAny<ChangePasswordDto>()))
                .ReturnsAsync(new ServiceResponse<User>(error: "Change failed"));

            var res = await _sut.ChangePassword(new ChangePasswordDto
            {
                CurrentPassword = "string1A?",
                NewPassword = "string1B?",
                RepeatNewPassword = "string1B?"
            });

            Assert.IsType<BadRequestResult>(res);
        }

        [Fact]
        public async Task ChangePassword_ChangeSuccess_ReturnsUser()
        {
            _userService.Setup(it => it.ChangeUserPassword(It.IsAny<long>(), It.IsAny<ChangePasswordDto>()))
                .ReturnsAsync(new ServiceResponse<User>(new User()));

            var res = await _sut.ChangePassword(new ChangePasswordDto
            {
                CurrentPassword = "string1A?",
                NewPassword = "string1B?",
                RepeatNewPassword = "string1B?"
            });

            Assert.NotNull(res);
            var okResult = res as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.IsType<UserDto>(okResult.Value);
        }
        #endregion
    }
}
