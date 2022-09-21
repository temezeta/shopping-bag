using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.DTOs.User;
using shopping_bag.Models;
using shopping_bag.Models.User;
using shopping_bag.Utility;

namespace shopping_bag.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        public AuthService(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<ServiceResponse<string>> Register(RegisterDto request)
        {
            var response = await _userService.GetUserByEmail(request.Email);
            if (response.IsSuccess)
            {
                return new ServiceResponse<string>(error: "User with email already exists");
            }


            if (!AuthHelper.ValidatePassword(request.Password, request.RepeatPassword))
            {
                return new ServiceResponse<string>(error: "Password does not meet requirements");
            }

            AuthHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var verificationToken = AuthHelper.CreateHexToken();

            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var user = new User
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,
                        Office = request.Office,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        VerificationToken = verificationToken
                    };
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        Role = "User"
                    });
                    _context.SaveChanges();
                    transaction.Commit();
                    return new ServiceResponse<string>(data: verificationToken);
                }
            } catch(Exception ex)
            {
                return new ServiceResponse<string>(error: ex.Message);
            }
        }

        public async Task<ServiceResponse<LoginData>> Login(LoginDto request)
        {
            var response = await _userService.GetUserByEmail(request.Email);

            if (!response.IsSuccess)
            {
                return new ServiceResponse<LoginData>(error: response.Error);
            }
            else if (response.Data.VerifiedAt == null){
                return new ServiceResponse<LoginData>(error: "User not verified");
            }
            else if(!AuthHelper.VerifyPasswordHash(request.Password, response.Data.PasswordHash, response.Data.PasswordSalt))
            {
                return new ServiceResponse<LoginData>(error: "Invalid password");
            }

            return new ServiceResponse<LoginData>(data: new LoginData
            {
                User = response.Data,
                LoginToken = AuthHelper.CreateToken(response.Data)
            });
        }

        public async Task<ServiceResponse<bool>> SetRefreshToken(User user)
        {
            var found = await _context.Users.FindAsync(user.Id);

            if (found == null)
            {
                return new ServiceResponse<bool>(error: "User not found");
            }

            found.RefreshToken = user.RefreshToken;
            found.TokenCreatedAt = user.TokenCreatedAt;
            found.TokenExpiresAt = user.TokenExpiresAt;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(data: true);
        }

        public async Task<ServiceResponse<bool>> VerifyUserToken(string verificationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == verificationToken);

            if(user == null)
            {
                return new ServiceResponse<bool>(error: "User not found");
            }

            user.VerifiedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(data: true);
        }

        public async Task<ServiceResponse<string>> SetPasswordResetToken(string email)
        {
            var resetToken = AuthHelper.CreateHexToken();
            var response = await _userService.GetUserByEmail(email);

            if (!response.IsSuccess)
            {
                return new ServiceResponse<string>(error: response.Error);
            }

            response.Data.PasswordResetToken = resetToken;
            response.Data.ResetTokenExpires = DateTime.Now.AddHours(2);
            await _context.SaveChangesAsync();
            return new ServiceResponse<string>(data: resetToken);
        }
        
        public async Task<ServiceResponse<bool>> ResetPassword(ResetPasswordDto request)
        {
            if (!AuthHelper.ValidatePassword(request.Password, request.RepeatPassword))
            {
                return new ServiceResponse<bool>(error: "Password does not meet requirements");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.ResetToken);

            if(user == null)
            {
                return new ServiceResponse<bool>(error: "User not found");
            }
            else if (user.ResetTokenExpires < DateTime.Now)
            {
                return new ServiceResponse<bool>(error: "Reset token expired");
            }

            AuthHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(data: true);
        }
    }
}
