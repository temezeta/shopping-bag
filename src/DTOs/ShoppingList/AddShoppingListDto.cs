using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;

namespace shopping_bag.DTOs.ShoppingList
{
    public class AddShoppingListDto
    {
        [Required]
        public string Name { get; set; }

        public string? Comment { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }
        [Required]
        public long OfficeId { get; set; }
        [Required]
        public long UserId { get; set; }
    }
}
