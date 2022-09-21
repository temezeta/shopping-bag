using shopping_bag.Models;
using shopping_bag.Models.User;

namespace shopping_bag.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<User>> GetUserByEmail(string email);
    }
}