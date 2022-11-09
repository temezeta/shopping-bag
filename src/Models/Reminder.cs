using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace shopping_bag.Models {
    public class Reminder {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public List<int> DueDaysBefore { get; set; }
        public List<int> ExpectedDaysBefore { get; set; }
        public long UserId { get; set; }
        public long ShoppingListId { get; set; }
        public ShoppingList ShoppingList { get; set; }
    }
}
