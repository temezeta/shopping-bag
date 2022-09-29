using shopping_bag.Filters;
using System.ComponentModel.DataAnnotations;

namespace shopping_bag.DTOs.User
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
