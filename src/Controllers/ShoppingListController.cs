using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Services;

namespace shopping_bag.Controllers
{
    public class ShoppingListController : BaseApiController
    {
        private readonly IShoppingListService _shoppingListService;
        public ShoppingListController(IUserService userService, IShoppingListService shoppingListService) : base(userService)
        {
            _shoppingListService = shoppingListService;
        }

        [HttpPost]
        [AllowAnonymous]
        //[Authorize(Roles = "Admin")]
        [Route("add")]
        public async Task<ActionResult> AddShoppingList([FromBody] AddShoppingListDto shoppingList)
        {
            var response = await _shoppingListService.AddShoppingList(shoppingList);

            if(!response.IsSuccess)
            {
                return BadRequest(response.Error);
            }
            return Ok(response.Data);
        }
    }
}