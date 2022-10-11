namespace shopping_bag.DTOs.ShoppingList
{
    public class ShoppingListDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string Comment { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime DeliveryDate { get; set; }

    }
}
