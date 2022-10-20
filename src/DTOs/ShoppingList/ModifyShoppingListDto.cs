namespace shopping_bag.DTOs.ShoppingList
{
    public class ModifyShoppingListDto
    {
        public string Name { get; set; }

        public string? Comment { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

    }
}
