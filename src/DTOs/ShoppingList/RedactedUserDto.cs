using shopping_bag.DTOs.Office;

namespace shopping_bag.DTOs.ShoppingList {
    public class RedactedUserDto {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public OfficeDto HomeOffice { get; set; }
    }
}
