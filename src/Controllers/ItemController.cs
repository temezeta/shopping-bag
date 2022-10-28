using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Services;

namespace shopping_bag.Controllers {

    public class ItemController : BaseApiController {

        private readonly IShoppingListService _shoppingListService;
        private readonly IMapper _mapper;
        public ItemController(IUserService userService, IShoppingListService shoppingListService, IMapper mapper) : base(userService) {
            _shoppingListService = shoppingListService ?? throw new ArgumentNullException(nameof(shoppingListService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<ItemDto>> AddItemToShoppingList([FromBody] AddItemDto itemToAdd, long listId) {
            itemToAdd.UserId = (await GetCurrentUser()).Id;
            itemToAdd.ShoppingListId = listId;

            var response = await _shoppingListService.AddItemToShoppingList(itemToAdd);

            if (!response.IsSuccess) {
                return BadRequest(response.Error);
            }
            return Ok(_mapper.Map<ItemDto>(response.Data));
        }

        [HttpDelete]
        [Route("{itemId}")]
        public async Task<ActionResult> RemoveItemFromShoppingList(long itemId) {
            var user = await GetCurrentUser();

            if (user == null) {
                return BadRequest();
            }

            var response = await _shoppingListService.RemoveItemFromShoppingList(user, itemId);

            if (!response.IsSuccess) {
                return BadRequest(response.Error);
            }
            return Ok();
        }

        [HttpPut]
        [Route("{itemId}")]
        public async Task<ActionResult<ItemDto>> ModifyItem([FromBody] ModifyItemDto itemToModify, long itemId) {
            var user = await GetCurrentUser();

            if (user == null) {
                return BadRequest();
            }

            var response = await _shoppingListService.ModifyItem(user, itemToModify, itemId);
            if(!response.IsSuccess) {
                return BadRequest(response.Error);
            }
            return Ok(_mapper.Map<ItemDto>(response.Data));
        }
    }
}
