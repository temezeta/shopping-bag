using shopping_bag.DTOs.User;
using shopping_bag.Models;
using shopping_bag.Models.User;

namespace shopping_bag.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<bool>> Register(RegisterDto request, string hexToken, string verificationBodyText);
        Task<ServiceResponse<LoginData>> Login(LoginDto request);
        Task<ServiceResponse<bool>> Logout(User user);
        Task<ServiceResponse<bool>> SetRefreshToken(User user);
        Task<ServiceResponse<bool>> VerifyUserToken(string verificationToken);
        Task<ServiceResponse<string>> SetPasswordResetToken(string email);
        Task<ServiceResponse<bool>> ResendVerificationEmail(string email, string hexToken, string verificationBodyText);
        Task<ServiceResponse<bool>> ResetPassword(ResetPasswordDto request);
    }
}