using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Models;

namespace shopping_bag.Services
{
    public interface IShoppingListService
    {
        Task<ServiceResponse<ShoppingList>> AddShoppingList(AddShoppingListDto shoppingList);
    }
}