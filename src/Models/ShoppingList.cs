using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace shopping_bag.Models
{
    public class ShoppingList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        public string? Comment { get; set; }

        public Boolean Ordered { get; set; }

        public Boolean Removed { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        public long OfficeId { get; set; }
        public Office ListDeliveryOffice { get; set; }

        public long? UserId { get; set; }
        public User.User? ListCreatorUser { get; set; }

        public List<Item> Items { get; set; } = new List<Item>();
    }
}
