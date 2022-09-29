using System.ComponentModel.DataAnnotations;

namespace shopping_bag.DTOs.User
{
    public class TokenResponseDto
    {
        [Required]
        public string Token { get; set; }
    }
}
