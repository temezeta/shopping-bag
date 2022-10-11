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

        public DateTime? DueDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

    }
}
