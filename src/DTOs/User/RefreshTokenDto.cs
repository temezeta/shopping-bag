using System.ComponentModel.DataAnnotations;

namespace shopping_bag.DTOs.User
{
    public class RefreshTokenDto
    {
        [Required]
        public string ExpiredToken { get; set; }
    }
}
