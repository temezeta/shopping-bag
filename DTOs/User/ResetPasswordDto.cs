
using System.ComponentModel.DataAnnotations;

namespace shopping_bag.DTOs.User
{
    public class ResetPasswordDto
    {
        [Required]
        public string ResetToken { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string RepeatPassword { get; set; }
    }
}
