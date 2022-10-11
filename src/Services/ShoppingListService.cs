using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using shopping_bag.Config;
using shopping_bag.DTOs.Office;
using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Models;
using shopping_bag.Models.User;

namespace shopping_bag.Services
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly AppDbContext _context;

        public ShoppingListService(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<ServiceResponse<ShoppingList>> AddShoppingList(AddShoppingListDto shoppingListData)
        {
            // Maybe check if an active list exists with same name? 
            if (_context.ShoppingLists.Any(s => s.Name == shoppingListData.Name))
            {
                return new ServiceResponse<ShoppingList>(error: "Shopping list with that name already exists.");
            }

            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var shoppingList = new ShoppingList
                    {
                        Name = shoppingListData.Name,
                        Comment = shoppingListData.Comment,
                        DueDate = shoppingListData.DueDate,
                        DeliveryDate = shoppingListData.DeliveryDate,
                    };
                    _context.ShoppingLists.Add(shoppingList);
                    _context.SaveChanges();
                    transaction.Commit();
                    return new ServiceResponse<ShoppingList>(data: shoppingList);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ShoppingList>(error: ex.Message);
            }
        }
    }
}
