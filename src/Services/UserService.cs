using Microsoft.EntityFrameworkCore;
using shopping_bag.Models.User;
using shopping_bag.Models;
using shopping_bag.Config;
using shopping_bag.DTOs.User;
using shopping_bag.Utility;
using shopping_bag.Models.Email;

namespace shopping_bag.Services {
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public UserService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<ServiceResponse<User>> GetUserByEmail(string email)
        {
            var user = await _context.Users.Include(u => u.UserRoles).Include(u => u.HomeOffice).Include(u => u.ReminderSettings).Include(u => u.Reminders).Include(u => u.ListReminderSettings).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.Removed)
            {
                return new ServiceResponse<User>(error: "User not found");
            }

            return new ServiceResponse<User>(data: user);
        }

        public async Task<ServiceResponse<User>> GetUserById(long id) {
            var user = await _context.Users.Include(u => u.UserRoles).Include(u => u.HomeOffice).Include(u => u.ReminderSettings).Include(u => u.Reminders).Include(u => u.ListReminderSettings).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null || user.Removed) {
                return new ServiceResponse<User>(error: "User not found");
            }

            return new ServiceResponse<User>(data: user);
        }

        public async Task<ServiceResponse<IEnumerable<User>>> GetUsers() {
            var users = await _context.Users.Include(u => u.UserRoles).Include(u => u.HomeOffice).Where(u => !u.Removed).ToListAsync();
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

        public async Task<ServiceResponse<User>> ModifyUser(User user, ModifyUserDto modifyData, long userId, string hexToken, string verificationBodyText)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var modifyUser = (await GetUserById(userId)).Data;

                    if (modifyUser == null || modifyUser.Removed)
                    {
                        return new ServiceResponse<User>(error: "User not found");
                    }

                    var isAdmin = user.UserRoles.Any(r => r.RoleName.Equals(Roles.AdminRole));

                    if (isAdmin && modifyData.RoleIds != null)
                    {
                        var roles = _context.UserRoles.Where(r => modifyData.RoleIds.Contains(r.RoleId)).ToList();

                        if (!roles.Any())
                        {
                            return new ServiceResponse<User>(error: "Roles not found");
                        }

                        modifyUser.UserRoles = roles;
                    }

                    if (!isAdmin && modifyUser.Id != user.Id)
                    {
                        return new ServiceResponse<User>(error: "You can only modify your own account");
                    }

                    var homeoffice = _context.Offices.FirstOrDefault(o => o.Id == modifyData.OfficeId && !o.Removed);

                    if (homeoffice == null)
                    {
                        return new ServiceResponse<User>(error: "Invalid office id");
                    }
                    modifyUser.FirstName = modifyData.FirstName;
                    modifyUser.LastName = modifyData.LastName;
                    modifyUser.OfficeId = modifyData.OfficeId;
                    modifyUser.HomeOffice = homeoffice;

                    if (modifyData.Email != modifyUser.Email)
                    {
                        modifyUser.Email = modifyData.Email;
                        modifyUser.VerificationToken = hexToken;
                        modifyUser.VerifiedAt = null;
                    }
                    _context.SaveChanges();

                    if (modifyUser.VerifiedAt == null)
                    {
                        var emailResponse = _emailService.SendEmail(new Email
                        {
                            To = modifyData.Email,
                            Subject = "Huld Shopping Bag - Account Verification",
                            Body = verificationBodyText
                        });

                        if (!emailResponse.IsSuccess)
                        {
                            return new ServiceResponse<User>(error: "Failed to send verification email");
                        }
                    }

                    transaction.Commit();
                    return new ServiceResponse<User>(data: modifyUser);
                }
            }
            catch (Exception)
            {
                return new ServiceResponse<User>(error: "Error in saving changes to database");
            }
        }

        public async Task<ServiceResponse<User>> ChangeUserPassword(long id, ChangePasswordDto request)
        {
            var user = (await GetUserById(id)).Data;

            if(user == null || user.Removed)
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
