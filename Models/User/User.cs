using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace shopping_bag.Models.User
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TokenCreatedAt { get; set; }
        public DateTime? TokenExpiresAt { get; set; }
        // Navigation properties
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}
