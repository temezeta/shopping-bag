using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shopping_bag.DTOs.ShoppingList;
using shopping_bag.Models;
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
        [Route("")]
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

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("")]
        public async Task<ActionResult<ShoppingListDto>> ModifyShoppingList([FromBody] ModifyShoppingListDto shoppingList, long shoppingListId)
        {
            var response = await _shoppingListService.ModifyShoppingList(shoppingList, shoppingListId);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Error);
            }
            return Ok(_mapper.Map<ShoppingListDto>(response.Data));
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("")]
        public async Task<ActionResult<bool>> RemoveShoppingList(long shoppingListId)
        {
            var response = await _shoppingListService.RemoveShoppingList(shoppingListId);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Error);
            }

            return Ok();
        }

        [HttpGet]
        [Route("byid")]
        public async Task<ActionResult<ShoppingListDto>> GetShoppingListById(long shoppingListId)
        {
            var response = await _shoppingListService.GetShoppingListById(shoppingListId);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Error);
            }

            return Ok(_mapper.Map<ShoppingListDto>(response.Data));
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<ShoppingListDto>>> GetShoppingListsByOffice(long officeId)
        {
            var response = await _shoppingListService.GetShoppingListsByOffice(officeId);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Error);
            }

            return Ok(_mapper.Map<IEnumerable<ShoppingListDto>>(response.Data));
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("order")]
        public async Task<ActionResult<bool>> OrderShoppingList(long shoppingListId)
        {
            var response = await _shoppingListService.OrderShoppingList(shoppingListId);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Error);
            }

            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("{listId}/order-amount")]
        public async Task<ActionResult> SetOrderedAmount([FromRoute] long listId, [FromBody] OrderedAmountDto request)
        {
            var response = await _shoppingListService.SetOrderedAmount(listId, request);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Error);
            }

            return Ok(_mapper.Map<ShoppingListDto>(response.Data));
        }

    }
}