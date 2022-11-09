using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Models;
using shopping_bag.Models.User;

namespace shopping_bag.Services
{
    public interface IShoppingListService
    {
        Task<ServiceResponse<ShoppingList>> AddShoppingList(AddShoppingListDto shoppingList);
        Task<ServiceResponse<ShoppingList>> ModifyShoppingList(ModifyShoppingListDto shoppingListData, long shoppingListId);
        Task<ServiceResponse<bool>> RemoveShoppingList(long shoppingListId);
        Task<ServiceResponse<Item>> AddItemToShoppingList(AddItemDto itemToAdd);
        Task<ServiceResponse<bool>> RemoveItemFromShoppingList(User user, long itemId);
        Task<ServiceResponse<Item>> ModifyItem(User user, ModifyItemDto itemToModify, long itemId);
        Task<ServiceResponse<ShoppingList>> GetShoppingListById(long shoppingListId);
        Task<ServiceResponse<bool>> UpdateLikeStatus(User user, long itemId, bool unlike);
        Task<ServiceResponse<IEnumerable<ShoppingList>>> GetShoppingListsByOffice(long officeId);
        Task<ServiceResponse<bool>> OrderShoppingList(long shoppingListId);
    }
}