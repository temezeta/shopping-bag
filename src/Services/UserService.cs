using Microsoft.EntityFrameworkCore;
using shopping_bag.Models.User;
using shopping_bag.Models;
using shopping_bag.Config;
using shopping_bag.DTOs.User;
using shopping_bag.Utility;

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
            var user = await _context.Users.Include(u => u.UserRoles).Include(u => u.HomeOffice).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return new ServiceResponse<User>(error: "User not found");
            }

            return new ServiceResponse<User>(data: user);
        }

        public async Task<ServiceResponse<User>> GetUserById(long id) {
            var user = await _context.Users.Include(u => u.UserRoles).Include(u => u.HomeOffice).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) {
                return new ServiceResponse<User>(error: "User not found");
            }

            return new ServiceResponse<User>(data: user);
        }

        public async Task<ServiceResponse<IEnumerable<User>>> GetUsers() {
            var users = await _context.Users.Include(u => u.UserRoles).Include(u => u.HomeOffice).ToListAsync();
            return new ServiceResponse<IEnumerable<User>>(users);
        }

        public async Task<ServiceResponse<bool>> RemoveUser(User user, long userId)
        {
            var removeUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (removeUser == null || removeUser.Removed)
            {
                return new ServiceResponse<bool>(error: "User not found");
            }

            var isAdmin = user.UserRoles.Any(r => r.RoleName.Equals(Roles.AdminRole));

            if (!isAdmin && user.Id != userId)
            {
                return new ServiceResponse<bool>(error: "You can only remove your own account");
            }

            removeUser.Removed = true;
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>(true);
        }

        public async Task<ServiceResponse<User>> ChangeUserPassword(long id, ChangePasswordDto request)
        {
            var user = await _context.Users.Include(u => u.UserRoles).Include(u => u.HomeOffice).FirstOrDefaultAsync(u => u.Id == id);

            if(user == null)
            {
                return new ServiceResponse<User>(error: "User not found");
            }
            else if(!AuthHelper.VerifyPasswordHash(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            {
                return new ServiceResponse<User>(error: "Invalid password");
            }

            AuthHelper.CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.SaveChangesAsync();

            return new ServiceResponse<User>(user);
        }
    }
}
