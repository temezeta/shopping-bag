using AutoMapper;
using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Models;
using shopping_bag.Models.User;

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
            var officeExists = _context.Offices.Any(office => office.Id == shoppingListData.OfficeId);

            if (!officeExists)
            {
                return new ServiceResponse<ShoppingList>(error: "Office doesn't exist");
            }
            // Check that there are no active shopping lists with the same name under the office
            if (_context.ShoppingLists.Where(s => s.OfficeId == shoppingListData.OfficeId && !s.Ordered && !s.Removed).Any(s => s.Name == shoppingListData.Name))
            {
                return new ServiceResponse<ShoppingList>(error: "Active shopping list with that name already exists.");
            }

            try
            {
                {
                    var shoppingList = new ShoppingList
                    {
                        Name = shoppingListData.Name,
                        Comment = shoppingListData.Comment,
                        Ordered = false,
                        Removed = false,
                        CreatedDate = DateTime.Now,
                        DueDate = shoppingListData.DueDate,
                        ExpectedDeliveryDate = shoppingListData.ExpectedDeliveryDate,
                        OfficeId = shoppingListData.OfficeId,
                        UserId = shoppingListData.UserId
                    };
                    _context.ShoppingLists.Add(shoppingList);
                    await _context.SaveChangesAsync();
                    return new ServiceResponse<ShoppingList>(data: shoppingList);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ShoppingList>(error: ex.Message);
            }
        }

        public async Task<ServiceResponse<ShoppingList>> ModifyShoppingList(ModifyShoppingListDto shoppingListData, long shoppingListId)
        {
            var shoppingList = await _context.ShoppingLists.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == shoppingListId);

            if (shoppingList == null || shoppingList.Removed)
            {
                return new ServiceResponse<ShoppingList>(error: "Invalid shoppingListId");
            }
            if (shoppingListData.DueDate != null && shoppingListData.DueDate != shoppingList.DueDate &&
                shoppingListData.DueDate < DateTime.Now)
            {
                return new ServiceResponse<ShoppingList>(error: "Shopping list due date passed");
            }
            if (shoppingListData.ExpectedDeliveryDate != null && shoppingListData.ExpectedDeliveryDate != shoppingList.ExpectedDeliveryDate &&
                shoppingListData.ExpectedDeliveryDate < DateTime.Now)
            {
                return new ServiceResponse<ShoppingList>(error: "Shopping list expected delivery date passed");
            }
            if (shoppingListData.Name != null && shoppingListData.Name != shoppingList.Name && 
                _context.ShoppingLists.Where(s => s.OfficeId == shoppingList.OfficeId && !s.Ordered).Any(s => s.Name == shoppingListData.Name))
            {
                return new ServiceResponse<ShoppingList>(error: "Active shopping list with that name already exists.");
            }

            _mapper.Map(shoppingListData, shoppingList);
            await _context.SaveChangesAsync();

            return new ServiceResponse<ShoppingList>(shoppingList);
        }

        public async Task<ServiceResponse<bool>> RemoveShoppingList(long shoppingListId)
        {
            var shoppingList = await _context.ShoppingLists.FirstAsync(s => s.Id == shoppingListId);

            if (shoppingList == null || shoppingList.Removed)
            {
                return new ServiceResponse<bool>(error: "Invalid shoppingListId");
            }

            shoppingList.Removed = true;
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>(true);
        }

        public async Task<ServiceResponse<IEnumerable<ShoppingList>>> GetShoppingListsByOffice(long officeId)
        {
            var office = await _context.Offices.FirstOrDefaultAsync(o => o.Id == officeId);

            if (office == null)
            {
                return new ServiceResponse<IEnumerable<ShoppingList>>(error: "Invalid officeId");
            }

            var shoppingLists = await _context.ShoppingLists.Include(s => s.Items).Where(s => s.OfficeId == officeId && !s.Removed).ToListAsync();
            return new ServiceResponse<IEnumerable<ShoppingList>>(shoppingLists);
        }

        #region Items
        public async Task<ServiceResponse<Item>> AddItemToShoppingList(AddItemDto itemToAdd) {
            if (string.IsNullOrEmpty(itemToAdd.Url) && string.IsNullOrEmpty(itemToAdd.Name)) {
                return new ServiceResponse<Item>(error: "Item url or name must be given");
            }
            var shoppingList = await _context.ShoppingLists.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == itemToAdd.ShoppingListId);
            if(shoppingList == null || shoppingList.Removed) {
                return new ServiceResponse<Item>(error: "Invalid shoppingListId");
            }
            if (shoppingList.Ordered) {
                return new ServiceResponse<Item>(error: "Shopping list already ordered");
            }
            if (shoppingList.DueDate != null && shoppingList.DueDate < DateTime.Now) {
                return new ServiceResponse<Item>(error: "Shopping list due date passed");
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

        public async Task<ServiceResponse<bool>> RemoveItemFromShoppingList(User user, long itemId) {
            var item = await _context.Items.Include(i => i.ShoppingList).FirstOrDefaultAsync(i => i.Id == itemId);
            if(item == null || item.ShoppingList.Removed) {
                return new ServiceResponse<bool>(error: "Item doesn't exist.");
            }
            var isAdmin = user.UserRoles.Any(r => r.RoleName.Equals(Roles.AdminRole));
            if(!isAdmin && (item.ShoppingList.DueDate < DateTime.Now || item.ShoppingList.Ordered)) {
                return new ServiceResponse<bool>(error: "Can't remove items from ordered lists");
            }
            if (!isAdmin && item.UserId != user.Id) {
                return new ServiceResponse<bool>(error: "You can only remove items added by you");
            }
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(true);
        }

        public async Task<ServiceResponse<Item>> ModifyItem(User user, ModifyItemDto itemToModify, long itemId) {
            var item = await _context.Items.Include(i => i.ShoppingList).FirstOrDefaultAsync(i => i.Id == itemId);
            if (item == null || item.ShoppingList.Removed) {
                return new ServiceResponse<Item>(error: "Item doesn't exist.");
            }
            var isAdmin = user.UserRoles.Any(r => r.RoleName.Equals(Roles.AdminRole));
            if(!isAdmin && user.Id != item.UserId) {
                return new ServiceResponse<Item>(error: "You can only modify items you have added");
            }
            if (!isAdmin && item.ShoppingList.Ordered) {
                return new ServiceResponse<Item>(error: "Shopping list already ordered");
            }
            if (!isAdmin && item.ShoppingList.DueDate != null && item.ShoppingList.DueDate < DateTime.Now) {
                return new ServiceResponse<Item>(error: "Shopping list due date passed");
            }
            if(string.IsNullOrEmpty(itemToModify.Name) && string.IsNullOrEmpty(itemToModify.Url)) {
                return new ServiceResponse<Item>(error: "Item must have a name or url");
            }
            if (!string.IsNullOrEmpty(itemToModify.Name) && item.ShoppingList.Items.Any(i => i.Id != item.Id && itemToModify.Name.ToLower().Equals(i.Name?.ToLower()))) {
                return new ServiceResponse<Item>(error: "Item with same name already in list");
            }
            if (!string.IsNullOrEmpty(itemToModify.Url) && item.ShoppingList.Items.Any(i => i.Id != item.Id && itemToModify.Url.ToLower().Equals(i.Url?.ToLower()))) {
                return new ServiceResponse<Item>(error: "Item with same url already in list");
            }
            if (!isAdmin && itemToModify.AmountOrdered != item.AmountOrdered) {
                return new ServiceResponse<Item>(error: "You can't modify amount ordered");
            }
            if (!isAdmin && itemToModify.IsChecked != item.IsChecked) {
                return new ServiceResponse<Item>(error: "You can't modify isChecked");
            }
            _mapper.Map(itemToModify, item);
            await _context.SaveChangesAsync();
            return new ServiceResponse<Item>(item);
        }
        #endregion
    }
}
