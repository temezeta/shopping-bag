using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace shopping_bag.Models {
    public class Item {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? ShopName { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsChecked { get; set; }
        public int AmountOrdered { get; set; }
        public long ShoppingListId { get; set; }
        public ShoppingList ShoppingList { get; set; }
        public long? UserId { get; set; }
        public User.User? ItemAdder { get; set; }
    }
}
