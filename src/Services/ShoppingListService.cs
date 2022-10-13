using AutoMapper;
using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Models;

namespace shopping_bag.Services
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ShoppingListService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<ShoppingList>> AddShoppingList(AddShoppingListDto shoppingListData)
        {
            // Maybe check if an active list exists with same name? 
            if (_context.ShoppingLists.Any(s => s.Name == shoppingListData.Name))
            {
                return new ServiceResponse<ShoppingList>(error: "Shopping list with that name already exists.");
            }

            var officeExists = _context.Offices.Any(office => office.Id == shoppingListData.OfficeId);

            if (!officeExists)
            {
                return new ServiceResponse<ShoppingList>(error: "Office doesn't exist");
            }

            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var shoppingList = new ShoppingList
                    {
                        Name = shoppingListData.Name,
                        Comment = shoppingListData.Comment,
                        Ordered = false,
                        CreatedDate = DateTime.Now,
                        StartDate = shoppingListData.StartDate,
                        DueDate = shoppingListData.DueDate,
                        ExpectedDeliveryDate = shoppingListData.ExpectedDeliveryDate,
                        OfficeId = shoppingListData.OfficeId,
                        UserId = shoppingListData.UserId
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

        public async Task<ServiceResponse<Item>> AddItemToShoppingList(AddItemDto itemToAdd) {
            if (string.IsNullOrEmpty(itemToAdd.Url) && string.IsNullOrEmpty(itemToAdd.Name)) {
                return new ServiceResponse<Item>(error: "Item url or name must be given");
            }
            var shoppingList = await _context.ShoppingLists.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == itemToAdd.ShoppingListId);
            if(shoppingList == null) {
                return new ServiceResponse<Item>(error: "Invalid shoppingListId");
            }
            if (shoppingList.Ordered) {
                return new ServiceResponse<Item>(error: "Shopping list already ordered");
            }
            if (shoppingList.DueDate != null && shoppingList.DueDate < DateTime.Now) {
                return new ServiceResponse<Item>(error: "Shopping list due date passed");
            }
            if (shoppingList.StartDate != null && shoppingList.StartDate > DateTime.Now) {
                return new ServiceResponse<Item>(error: "Shopping list not open yet");
            }
            if(!string.IsNullOrEmpty(itemToAdd.Name) && shoppingList.Items.Any(i => itemToAdd.Name.ToLower().Equals(i.Name?.ToLower()))) {
                return new ServiceResponse<Item>(error: "Item with same name already in list");
            }
            if (!string.IsNullOrEmpty(itemToAdd.Url) && shoppingList.Items.Any(i => itemToAdd.Url.ToLower().Equals(i.Url?.ToLower()))) {
                return new ServiceResponse<Item>(error: "Item with same url already in list");
            }
            var item = _mapper.Map<Item>(itemToAdd);
            shoppingList.Items.Add(item);
            await _context.SaveChangesAsync();

            return new ServiceResponse<Item>(item);
        }
    }
}
