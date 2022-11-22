using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.Models;
using shopping_bag.Models.User;

namespace shopping_bag.Services {
    public class UserRoleService : IUserRoleService {

        private readonly AppDbContext _context;

        public UserRoleService(AppDbContext context) {
            _context = context;
        }

        public async Task<ServiceResponse<IEnumerable<UserRole>>> GetUserRoles() {
            return new ServiceResponse<IEnumerable<UserRole>>(await _context.UserRoles.ToListAsync());
        }
    }
}
