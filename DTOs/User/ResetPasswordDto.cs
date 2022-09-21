
namespace shopping_bag.DTOs.User
{
    public class ResetPasswordDto
    {
        public string ResetToken { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
    }
}
