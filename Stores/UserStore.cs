using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.Models.User;
namespace shopping_bag.Stores
{
    public class UserStore : IUserStore
    {
        private AppDbContext _context;
        public UserStore(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddUser(NewUser user)
        {
            try
            {
                _context.Users.Add(new User
                {
                    Email = user.Email,
                    PasswordHash = user.PasswordHash,
                    PasswordSalt = user.PasswordSalt
                });
                await _context.SaveChangesAsync();
                return true;
            } catch
            {
                return false;
            }
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> SetRefreshToken(User user)
        {
            var found = await _context.Users.FindAsync(user.Id);
            if(found == null)
            {
                return false;
            }

            found.RefreshToken = user.RefreshToken;
            found.TokenCreatedAt = user.TokenCreatedAt;
            found.TokenExpiresAt = user.TokenExpiresAt;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
