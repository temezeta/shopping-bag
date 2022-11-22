using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using shopping_bag.DTOs.Reminder;
using shopping_bag.DTOs.User;
using shopping_bag.Services;

namespace shopping_bag.Controllers {

    public class ReminderController : BaseApiController {

        private readonly IReminderService _reminderService;
        private readonly IMapper _mapper;

        public ReminderController(IUserService userService, IReminderService reminderService, IMapper mapper) : base(userService) {
            _reminderService = reminderService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> SetGlobalReminderSettings([FromBody] ReminderSettingsDto settings) {
            var currentUser = await GetCurrentUser();
            if (currentUser == null) {
                return BadRequest();
            }
            var resp = await _reminderService.SetGlobalReminderSettings(currentUser.Id, settings);
            if(!resp.IsSuccess) {
                return BadRequest(resp.Error);
            }
            return _mapper.Map<UserDto>(resp.Data.User);
        }

        [HttpPost]
        [Route("list")]
        public async Task<ActionResult<UserDto>> SetListReminder([FromBody] ListReminderSettingsDto settings, [FromQuery] long listId) {
            var currentUser = await GetCurrentUser();

            if(currentUser == null) {
                return BadRequest();
            }

            settings.ShoppingListId = listId;
            var resp = await _reminderService.SetListReminder(currentUser.Id, settings, listId);
            if (!resp.IsSuccess) {
                return BadRequest(resp.Error);
            }

            return _mapper.Map<UserDto>(resp.Data);
        }
    }
}
