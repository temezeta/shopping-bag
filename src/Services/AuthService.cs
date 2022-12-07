using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using shopping_bag.Config;
using shopping_bag.DTOs.User;
using shopping_bag.Models;
using shopping_bag.Models.Email;
using shopping_bag.Models.User;
using shopping_bag.Utility;

namespace shopping_bag.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public AuthService(AppDbContext context, IUserService userService, IEmailService emailService)
        {
            _context = context;
            _userService = userService;
            _emailService = emailService;
        }

        public async Task<ServiceResponse<bool>> Register(RegisterDto request, string hexToken, string verificationBodyText)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var response = await _userService.GetUserByEmail(request.Email);
                    if (response.IsSuccess)
                    {
                        return new ServiceResponse<bool>(error: "User with email already exists");
                    }

                    AuthHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

                    var officeExists = _context.Offices.Any(office => office.Id == request.OfficeId);

                    if (!officeExists)
                    {
                        return new ServiceResponse<bool>(error: "Office doesn't exist");
                    }

                    var defaultRole = _context.UserRoles.FirstOrDefault(r => r.RoleName.Equals(Roles.DefaultRole));
                    if (defaultRole == null)
                    {
                        // TODO: Shouldn't happen ever because roles should always exist.
                        // Throw exception or something instead?
                        return new ServiceResponse<bool>(error: "User role doesn't exist in database.");
                    }

                    var user = new User
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,
                        OfficeId = request.OfficeId,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        VerificationToken = hexToken
                    };
                    user.UserRoles.Add(defaultRole);
                    var reminderSettings = new ReminderSettings() {
                        ReminderDaysBeforeDueDate = new List<int>() { 2 },
                        ReminderDaysBeforeExpectedDate = new List<int>(),
                        DueDateRemindersDisabled = false,
                        ExpectedRemindersDisabled = true,
                        UserId = user.Id,
                        AllRemindersDisabled = false
                    };
                    user.ReminderSettings = reminderSettings;
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    var emailResponse = _emailService.SendEmail(new Email
                    {
                        To = request.Email,
                        Subject = "Huld Shopping Bag - Account Verification",
                        Body = verificationBodyText
                    });

                    if (!emailResponse.IsSuccess)
                    {
                        return new ServiceResponse<bool>(error: "Failed to send verification email");
                    }                    

                    transaction.Commit();
                    return new ServiceResponse<bool>(true);
                }
            } catch(Exception ex)
            {
                return new ServiceResponse<bool>(error: ex.Message);
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

        public async Task<ServiceResponse<bool>> Logout(User user)
        {
            var found = await _context.Users.FindAsync(user.Id);

            if (found == null)
            {
                return new ServiceResponse<bool>(error: "User not found");
            }

            found.RefreshToken = null;
            found.TokenCreatedAt = null;
            found.TokenExpiresAt = null;
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>(data: true);
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
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var resetToken = AuthHelper.CreateHexToken();
                    var response = await _userService.GetUserByEmail(email, false);

                    if (!response.IsSuccess)
                    {
                        return new ServiceResponse<string>(error: response.Error);
                    }

                    response.Data.PasswordResetToken = resetToken;
                    response.Data.ResetTokenExpires = DateTime.Now.AddHours(2);
                    _context.SaveChanges();

                    var recoveryBodytext = string.Format(StaticConfig.RecoveryEmailBodyText, resetToken);
                    var emailResponse = _emailService.SendEmail(new Email
                    {
                        To = email,
                        Subject = "Huld Shopping Bag - Account Recovery",
                        Body = recoveryBodytext
                    });

                    if (!emailResponse.IsSuccess)
                    {
                        return new ServiceResponse<string>(error: "Failed to send verification email");
                    }

                    transaction.Commit();
                    return new ServiceResponse<string>(data: resetToken);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(error: ex.Message);
            }
        }
        
        public async Task<ServiceResponse<bool>> ResetPassword(ResetPasswordDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.ResetToken);

            if(user == null)
            {
                return new ServiceResponse<bool>(error: "User not found");
            }
            else if (user.ResetTokenExpires < DateTime.Now)
            {
                return new ServiceResponse<bool>(error: "Reset token expired");
            }
            else if (user.Disabled)
            {
                user.Disabled = false;
                user.VerifiedAt = DateTime.Now;
            }

            AuthHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(data: true);
        }

        public async Task<ServiceResponse<bool>> ResendVerificationEmail(string email, string hexToken, string verificationBodyText)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var response = await _userService.GetUserByEmail(email);

                    if (!response.IsSuccess)
                    {
                        return new ServiceResponse<bool>(error: "User not found");
                    }

                    var user = response.Data;

                    if (user.VerifiedAt != null)
                    {
                        return new ServiceResponse<bool>(error: "User is already verified");
                    }

                    user.VerificationToken = hexToken;
                    await _context.SaveChangesAsync();

                    var emailResponse = _emailService.SendEmail(new Email
                    {
                        To = email,
                        Subject = "Huld Shopping Bag - Account Verification",
                        Body = verificationBodyText
                    });

                    if (!emailResponse.IsSuccess)
                    {
                        return new ServiceResponse<bool>(error: "Failed to send verification email");
                    }

                    transaction.Commit();
                    return new ServiceResponse<bool>(true);
                }
            } catch (Exception ex)
            {
                return new ServiceResponse<bool>(error: ex.Message);
            }
        }
    }
}
