using shopping_bag.Models;
using shopping_bag.Models.User;

namespace shopping_bag.Services {
    public interface IUserRoleService {

        Task<ServiceResponse<IEnumerable<UserRole>>> GetUserRoles();
    }
}
