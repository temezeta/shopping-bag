using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace shopping_bag.DTOs.User {
    public class UserRoleDto {
        public long RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
