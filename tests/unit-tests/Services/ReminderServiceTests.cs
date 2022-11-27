using Moq;
using shopping_bag.Config;
using shopping_bag.DTOs.Reminder;
using shopping_bag.Models;
using shopping_bag.Models.User;
using shopping_bag.Services;

namespace shopping_bag_unit_tests.Services {
    public class ReminderServiceTests : BaseServiceTest {

        private readonly AppDbContext _context;
        private readonly ReminderService _sut;
        private readonly Mock<IEmailService> _emailService = new Mock<IEmailService>();
        private readonly Mock<IUserService> _userService = new Mock<IUserService>();

        public ReminderServiceTests() : base() {
            _context = GetDatabase();
            _sut = new ReminderService(_context, _userService.Object, UnitTestHelper.GetMapper(), _emailService.Object);
        }

        [Fact]
        public async Task SetGlobalReminderSettings_ValidSettings_ReminderSettingsReturned() {
            _userService.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(new ServiceResponse<User>(NormalUser));
            var settings = new ReminderSettingsDto() {
                AllRemindersDisabled = false,
                DueDateRemindersDisabled = false,
                ReminderDaysBeforeDueDate = new List<int>() { 1 },
                ExpectedRemindersDisabled = false,
                ReminderDaysBeforeExpectedDate = new List<int>() { 1 }
            };
            var response = await _sut.SetGlobalReminderSettings(NormalUser.Id, settings);
            Assert.True(response.IsSuccess);
            Assert.Equal(settings.AllRemindersDisabled, response.Data.AllRemindersDisabled);
            Assert.Equal(settings.DueDateRemindersDisabled, response.Data.DueDateRemindersDisabled);
            Assert.Equal(settings.ReminderDaysBeforeDueDate, response.Data.ReminderDaysBeforeDueDate);
            Assert.Equal(settings.ExpectedRemindersDisabled, response.Data.ExpectedRemindersDisabled);
            Assert.Equal(settings.ReminderDaysBeforeExpectedDate, response.Data.ReminderDaysBeforeExpectedDate);
        }

        [Fact]
        public async Task SetListReminder_ValidSettings_ReminderCreated() {
            _userService.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(new ServiceResponse<User>(NormalUser));
            var settings = new ListReminderSettingsDto() {
                DueDateRemindersDisabled = false,
                ReminderDaysBeforeDueDate = new List<int>() { 1 },
                ExpectedRemindersDisabled = false,
                ReminderDaysBeforeExpectedDate = new List<int>() { 1 },
                ShoppingListId = NormalList.Id
            };
            var response = await _sut.SetListReminder(NormalUser.Id, settings, NormalList.Id);
            Assert.True(response.IsSuccess);
            var reminder = response.Data.Reminders.FirstOrDefault(r => r.ShoppingListId == NormalList.Id);
            Assert.NotNull(reminder);
        }

        [Fact]
        public async Task SetListReminder_IntervalsPassed_ReminderNotCreatedSettingsRemoved() {
            _userService.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(new ServiceResponse<User>(NormalUser));
            var settings = new ListReminderSettingsDto() {
                DueDateRemindersDisabled = false,
                ReminderDaysBeforeDueDate = new List<int>() { 3 },
                ExpectedRemindersDisabled = false,
                ReminderDaysBeforeExpectedDate = new List<int>() { 3 },
                ShoppingListId = NormalList.Id
            };
            var response = await _sut.SetListReminder(NormalUser.Id, settings, NormalList.Id);
            Assert.True(response.IsSuccess);
            var reminder = response.Data.Reminders.FirstOrDefault(r => r.ShoppingListId == NormalList.Id);
            Assert.Null(reminder);
            var listSettings = response.Data.ListReminderSettings.FirstOrDefault(r => r.ShoppingListId == NormalList.Id);
            Assert.Null(listSettings);
        }

        [Theory]
        [InlineData(new[] { 1, 2, 3 })]
        [InlineData(new[] { 3, 1, 2 })]
        [InlineData(new[] { 1 })]
        [InlineData(new int[] { })]
        [InlineData(null)]
        public void IsValidReminderInterval_Valid_ReturnsTrue(int[] interval) {
            var result = _sut.IsValidReminderInterval(interval?.ToList());
            Assert.True(result);
        }

        [Theory]
        [InlineData(new[] { 1, 1, 2 })]
        [InlineData(new[] { 1, 0, 2 })]
        [InlineData(new[] { 1, -2, 3 })]
        public void IsValidReminderInterval_Invalid_ReturnsFalse(int[] interval) {
            var result = _sut.IsValidReminderInterval(interval?.ToList());
            Assert.False(result);
        }

        public static IEnumerable<object> TrimIntervalData_NotModified() {
            yield return new object[] { new List<int>() { 1, 2, 3 }, DateTime.Now.AddDays(7) };
            yield return new object[] { new List<int>() { 1, 2, 3 }, DateTime.Now.AddDays(3).AddSeconds(1) };
            yield return new object[] { new List<int>() { 2 }, DateTime.Now.AddDays(3) };
            yield return new object[] { new List<int>() { 1, 2, 3 }, null };
        }
        public static IEnumerable<object> TrimIntervalData_Modified() {
            yield return new object[] { new List<int>() { 1, 2, 3 }, DateTime.Now.AddDays(2).AddSeconds(1), new List<int>() { 1, 2 } };
            yield return new object[] { new List<int>() { 1, 2, 3 }, DateTime.Now.AddDays(1).AddSeconds(1), new List<int>() { 1 } };
            yield return new object[] { new List<int>() { 1, 2, 3 }, DateTime.Now, new List<int>() { } };
        }

        [Theory]
        [MemberData(nameof(TrimIntervalData_NotModified))]
        public void TrimInterval_ResultNotModified(List<int> interval, DateTime? targetDate) {
            var result = _sut.TrimInterval(interval, targetDate);
            Assert.Equal(interval, result);
        }

        [Theory]
        [MemberData(nameof(TrimIntervalData_Modified))]
        public void TrimInterval_ResultModified(List<int> interval, DateTime? targetDate, List<int> expected) {
            var result = _sut.TrimInterval(interval, targetDate);
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object> IsDateWithinReminderIntervalData_True() {
            yield return new object[] { DateTime.Now.AddDays(2), 2 };
            yield return new object[] { DateTime.Now.AddDays(1), 1 };
            yield return new object[] { DateTime.Now.AddDays(1), 2 };
        }
        public static IEnumerable<object> IsDateWithinReminderIntervalData_False() {
            yield return new object[] { DateTime.Now.AddDays(2), 1 };
            yield return new object[] { DateTime.Now.AddDays(1).AddHours(1), 1 };
            yield return new object[] { null, 3 };
        }

        [Theory]
        [MemberData(nameof(IsDateWithinReminderIntervalData_True))]
        public void IsDateWithinReminderInterval_True(DateTime? date, int daysBefore) {
            var result = _sut.IsDateWithinReminderInterval(date, daysBefore);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(IsDateWithinReminderIntervalData_False))]
        public void IsDateWithinReminderInterval_False(DateTime? date, int daysBefore) {
            var result = _sut.IsDateWithinReminderInterval(date, daysBefore);
            Assert.False(result);
        }
    }
}
