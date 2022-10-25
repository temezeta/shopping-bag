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
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        // Refresh token
        public string? RefreshToken { get; set; }
        public DateTime? TokenCreatedAt { get; set; }
        public DateTime? TokenExpiresAt { get; set; }
        // Email verification
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        // Password reset
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        // Navigation properties
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public long OfficeId { get; set; }
        public Office HomeOffice { get; set; }
        public List<Item> LikedItems { get; set; } = new List<Item>();

    }
}
