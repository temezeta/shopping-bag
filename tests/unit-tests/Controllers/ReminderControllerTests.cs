using Moq;
using shopping_bag.Controllers;
using shopping_bag.DTOs.User;
using shopping_bag.Models.User;
using shopping_bag.Models;
using shopping_bag.Services;
using shopping_bag.DTOs.Reminder;

namespace shopping_bag_unit_tests.Controllers {
    public class ReminderControllerTests : BaseControllerTest {

        private ReminderController _sut;
        private readonly Mock<IReminderService> _reminderService = new Mock<IReminderService>();
        private readonly Mock<IUserService> _userService = new Mock<IUserService>();

        public ReminderControllerTests() : base() {
            var user= new ServiceResponse<User>(data: new User());
            _userService.Setup(x => x.GetUserByEmail(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(user);
            _sut = new ReminderController(_userService.Object, _reminderService.Object, UnitTestHelper.GetMapper()) {
                ControllerContext = GetLoggedInControllerContext()
            };
        }


        [Fact]
        public async void SetGlobalReminderSettings_Ok_ReturnsUserDto() {
            var reminderResponse = new ServiceResponse<ReminderSettings>(new ReminderSettings() { User = new User() });
            _reminderService.Setup(x => x.SetGlobalReminderSettings(It.IsAny<long>(), It.IsAny<ReminderSettingsDto>())).ReturnsAsync(reminderResponse);
            var result = await _sut.SetGlobalReminderSettings(new ReminderSettingsDto());
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.IsType<UserDto>(result.Value);
        }

        [Fact]
        public async void SetListReminderSettings_Ok_ReturnsUserDto() {
            var reminderResponse = new ServiceResponse<User>(new User());
            _reminderService.Setup(x => x.SetListReminder(It.IsAny<long>(), It.IsAny<ListReminderSettingsDto>(), It.IsAny<long>())).ReturnsAsync(reminderResponse);
            var result = await _sut.SetListReminder(new ListReminderSettingsDto(), 0);
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.IsType<UserDto>(result.Value);
        }
    }
}
