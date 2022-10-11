using Moq;
using shopping_bag.Config;
using shopping_bag.Services;

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

    }
}
