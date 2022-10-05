using shopping_bag.Filters;
using System.ComponentModel.DataAnnotations;

namespace shopping_bag.DTOs.User
{
    public class RegisterDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [ValidateEmail]
        public string Email { get; set; }
        [Required]
        public long OfficeId { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string RepeatPassword { get; set; }
    }
}
