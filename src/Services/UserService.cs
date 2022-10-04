using Microsoft.EntityFrameworkCore;
using shopping_bag.Models.User;
using shopping_bag.Models;
using shopping_bag.Config;

namespace shopping_bag.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<User>> GetUserByEmail(string email)
        {
            var user = await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return new ServiceResponse<User>(error: "User not found");
            }

            return new ServiceResponse<User>(data: user);
        }

    }
}
