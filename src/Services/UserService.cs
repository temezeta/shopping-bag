using Microsoft.EntityFrameworkCore;
using shopping_bag.Models.User;
using shopping_bag.Models;
using shopping_bag.Config;

namespace shopping_bag.Services {
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<User>> GetUserByEmail(string email)
        {
            var user = await _context.Users.Include(u => u.UserRoles).Include(u => u.HomeOffice).Include(u => u.ReminderSettings).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return new ServiceResponse<User>(error: "User not found");
            }

            return new ServiceResponse<User>(data: user);
        }

        public async Task<ServiceResponse<User>> GetUserById(long id) {
            var user = await _context.Users.Include(u => u.UserRoles).Include(u => u.HomeOffice).Include(u => u.ReminderSettings).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) {
                return new ServiceResponse<User>(error: "User not found");
            }

            return new ServiceResponse<User>(data: user);
        }

        public async Task<ServiceResponse<IEnumerable<User>>> GetUsers() {
            var users = await _context.Users.Include(u => u.UserRoles).Include(u => u.HomeOffice).ToListAsync();
            return new ServiceResponse<IEnumerable<User>>(users);
        }
    }
}
