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

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _userStore.GetUserByEmail(email);

            if(user != null)
            {
                user.Roles = await _userStore.GetUserRoles(user.Id);
            }

            return user;
        }

        public async Task<bool> SetRefreshToken(User user)
        {
            return await _userStore.SetRefreshToken(user);
        }
    }
}
