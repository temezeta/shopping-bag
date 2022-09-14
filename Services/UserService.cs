using shopping_bag.Models.User;
using shopping_bag.Stores;

namespace shopping_bag.Services
{
    public class UserService : IUserService
    {
        private readonly IUserStore _userStore;
        public UserService(IUserStore userStore)
        {
            _userStore = userStore;
        }
        public async Task<bool> AddUser(NewUser user)
        {
            return await _userStore.AddUser(user);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userStore.GetUserByEmail(email);
        }

        public async Task<bool> SetRefreshToken(User user)
        {
            return await _userStore.SetRefreshToken(user);
        }
    }
}
