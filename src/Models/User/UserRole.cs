using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace shopping_bag.Models.User
{
    public class UserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserRoleId { get; set; }
        [Required]
        public string Role { get; set; }
        // Navigation properties
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
        public User User { get; set; }
    }
}
