using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using shopping_bag.DTOs.Reminder;
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
        public async Task<ActionResult<ReminderSettingsDto>> SetGlobalReminder([FromBody] ReminderSettingsDto settings) {
            var currentUser = await GetCurrentUser();
            var resp = await _reminderService.SetReminder(currentUser.Id, settings);
            if(!resp.IsSuccess) {
                return BadRequest(resp.Error);
            }
            return _mapper.Map<ReminderSettingsDto>(resp.Data);
        }

        [HttpPost]
        [Route("list")]
        public async Task<ActionResult<ReminderSettingsDto>> SetListReminder([FromBody] ReminderSettingsDto settings, long listId) {
            var currentUser = await GetCurrentUser();
            var resp = await _reminderService.SetListReminder(currentUser.Id, settings, listId);
            if (!resp.IsSuccess) {
                return BadRequest(resp.Error);
            }
            return _mapper.Map<ReminderSettingsDto>(resp.Data);
        }
    }
}
