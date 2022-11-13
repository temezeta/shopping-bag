using System.ComponentModel.DataAnnotations;

namespace shopping_bag.DTOs.User
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
        public string NewPassword { get; set; }
        [Required]
        [Compare(nameof(NewPassword))]
        public string RepeatNewPassword { get; set; }
    }
}
