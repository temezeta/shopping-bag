using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace shopping_bag.Models.User
{

    public class UserRole {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RoleId { get; set; }
        public string RoleName { get; set; }

        public IEnumerable<User> Users { get; set; } = new List<User>();
    }
}
