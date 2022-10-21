using shopping_bag.Models;

namespace shopping_bag.DTOs.ShoppingList
{
    public class ShoppingListDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string? Comment { get; set; }

        public Boolean Ordered { get; set; }

        public Boolean Removed { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        public List<ItemDto> Items { get; set; } = new List<ItemDto>();
    }
}
