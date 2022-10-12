using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Services;

namespace shopping_bag.Controllers
{
    public class ShoppingListController : BaseApiController
    {
        private readonly IShoppingListService _shoppingListService;
        private readonly IMapper _mapper;

        public ShoppingListController(IUserService userService, IShoppingListService shoppingListService, IMapper mapper) : base(userService)
        {
            _shoppingListService = shoppingListService ?? throw new ArgumentNullException(nameof(shoppingListService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("add")]
        public async Task<ActionResult<ShoppingListDto>> AddShoppingList([FromBody] AddShoppingListDto shoppingList)
        {
            shoppingList.UserId = (await GetCurrentUser()).Id;
            var response = await _shoppingListService.AddShoppingList(shoppingList);

            if(!response.IsSuccess)
            {
                return BadRequest(response.Error);
            }
            return Ok(_mapper.Map<ShoppingListDto>(response.Data));
        }
    }
}