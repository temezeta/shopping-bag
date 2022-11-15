using System.ComponentModel.DataAnnotations;

namespace shopping_bag.DTOs.ShoppingList
{
    public class OrderedAmountDto
    {
        [Required]
        public int ItemId { get; set; }
        [Required]
        public int AmountOrdered { get; set; }
    }
}
