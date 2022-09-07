using shopping_bag.Models.User;

namespace shopping_bag.Stores
{
    // Dummy user store to represent a database
    public class DummyUserStore : IUserStore
    {
        private static User CurrentUser { get; set; }

        public async Task<bool> AddUser(NewUser user)
        {
            CurrentUser = new User
            {
                Id = 1,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,
            };
            return true;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return CurrentUser;
        }

        public async Task<bool> SetRefreshToken(User user)
        {
            CurrentUser = user;
            return true;
        }

        public async Task<List<UserRole>> GetUserRoles(long userId)
        {
            return new List<UserRole>
            {
                new UserRole
                {
                    UserId = 1,
                    Role = "User"
                },
                new UserRole
                {
                    UserId = 1,
                    Role = "Admin"
                }
            };
        }
    }
}
