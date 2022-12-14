namespace shopping_bag.DTOs.ShoppingList
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? ShopName { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsChecked { get; set; }
        public int AmountOrdered { get; set; }
        public long ShoppingListId { get; set; }
        public RedactedUserDto? ItemAdder { get; set; }
        public IEnumerable<RedactedUserDto> UsersWhoLiked { get; set; } = new List<RedactedUserDto>();
    }
}
