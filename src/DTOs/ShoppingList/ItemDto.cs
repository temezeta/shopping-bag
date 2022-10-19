namespace shopping_bag.DTOs.ShoppingList
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? ShopName { get; set; }
        public string? Comment { get; set; }
        public bool IsChecked { get; set; }
        public int AmountOrdered { get; set; }
        public ItemAdderUserDto? ItemAdder { get; set; }
    }
}
