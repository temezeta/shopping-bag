using shopping_bag.DTOs.User;
using shopping_bag.Models;
using shopping_bag.Models.User;

namespace shopping_bag.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<User>> GetUserByEmail(string email);
        Task<ServiceResponse<User>> GetUserById(long id);
        Task<ServiceResponse<IEnumerable<User>>> GetUsers();
        Task<ServiceResponse<bool>> DisableUser(User user, long userId);
        Task<ServiceResponse<User>> ModifyUser(User user, ModifyUserDto modifyData, long userId, string hexToken, string verificationBodyText);
        Task<ServiceResponse<User>> ChangeUserPassword(long id, ChangePasswordDto request);
    }
}