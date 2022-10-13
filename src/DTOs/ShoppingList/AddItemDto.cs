namespace shopping_bag.DTOs.ShoppingList {
    public class AddItemDto {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? ShopName { get; set; }
        public string? Comment { get; set; }
        public long ShoppingListId { get; set; }
        public long? UserId { get; set; }
    }
}
