using shopping_bag.Models.User;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using shopping_bag.DTOs.Office;

namespace shopping_bag.DTOs.User
{
    public class ModifyUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<long>? RoleIds { get; set; }
        public long OfficeId { get; set; }
    }
}
