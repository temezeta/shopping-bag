using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shopping_bag.DTOs.User;
using shopping_bag.Services;

namespace shopping_bag.Controllers {
    public class UserController : BaseApiController {

        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper) : base(userService) {
            _mapper = mapper;
        }

        [HttpGet]
        [Route("list")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUserList() {
            var result = await _userService.GetUsers();
            if(!result.IsSuccess) {
                return BadRequest(result.Error);
            }

            var users = _mapper.Map<IEnumerable<UserDto>>(result.Data);
            return Ok(users);
        }

        [HttpGet]
        [Route("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> GetUserById([FromRoute] long userId) {
            var result = await _userService.GetUserById(userId);
            if (!result.IsSuccess) {
                return BadRequest(result.Error);
            }

            var user = _mapper.Map<UserDto>(result.Data);
            return user;
        }

        [HttpGet]
        [Route("me")]
        public async Task<ActionResult<UserDto>> GetLoggedInUser() {
            var user = await GetCurrentUser();
            if (user == null) {
                return BadRequest();
            }

            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        [HttpPut]
        [Route("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var user = await GetCurrentUser();

            if(user == null)
            {
                return BadRequest();
            }

            var response = await _userService.ChangeUserPassword(user.Id, request);

            if (!response.IsSuccess)
            {
                return BadRequest();
            }

            return Ok(_mapper.Map<UserDto>(response.Data));
        }
    }
}
