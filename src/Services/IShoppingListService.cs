using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Models;
using shopping_bag.Models.User;

namespace shopping_bag.Services
{
    public interface IShoppingListService
    {
        Task<ServiceResponse<ShoppingList>> AddShoppingList(AddShoppingListDto shoppingList);
        Task<ServiceResponse<Item>> AddItemToShoppingList(AddItemDto itemToAdd);
        Task<ServiceResponse<bool>> RemoveItemFromShoppingList(User user, long itemId);
        Task<ServiceResponse<Item>> ModifyItem(User user, ModifyItemDto itemToModify, long itemId);
        Task<ServiceResponse<IEnumerable<ShoppingList>>> GetShoppingListsByOffice(long officeId);
    }
}