using MimeKit.Cryptography;
using System.ComponentModel.DataAnnotations;

namespace shopping_bag.DTOs.ShoppingList
{
    public class CheckedItemDto
    {
        [Required]
        public long ItemId { get; set; }

        [Required]
        public bool IsChecked { get; set; }
    }
}
