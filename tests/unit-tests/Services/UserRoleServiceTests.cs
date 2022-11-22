using shopping_bag.Config;
using shopping_bag.Services;

namespace shopping_bag_unit_tests.Services {
    public class UserRoleServiceTests : BaseServiceTest {

        private readonly AppDbContext _context;
        private readonly UserRoleService _sut;

        public UserRoleServiceTests() : base() {
            _context = GetDatabase();
            _sut = new UserRoleService(_context);
        }

        [Fact]
        public async Task GetUserRoles_ListReturned() {
            var response = await _sut.GetUserRoles();
            Assert.True(response.IsSuccess);
            Assert.Equal(UserRoles.Count, response.Data.Count());
        }
    }
}
