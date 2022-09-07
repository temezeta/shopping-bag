using shopping_bag.Models.User;

namespace shopping_bag.Stores
{
    public interface IUserStore
    {
        Task<bool> AddUser(NewUser user);
        Task<User> GetUserByEmail(string email);
        Task<bool> SetRefreshToken(User user);
        Task<List<UserRole>> GetUserRoles(long userId);
    }
}