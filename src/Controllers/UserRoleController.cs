using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shopping_bag.DTOs.User;
using shopping_bag.Services;

namespace shopping_bag.Controllers {
    public class UserRoleController : BaseApiController {

        private readonly IMapper _mapper;
        private readonly IUserRoleService _roleService;
        public UserRoleController(IUserService userService, IUserRoleService roleService, IMapper mapper) : base(userService) {
            _mapper = mapper;
            _roleService = roleService;
        }

        [HttpGet]
        [Route("list")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetUserRoleList() {
            var result = await _roleService.GetUserRoles();
            if (!result.IsSuccess) {
                return BadRequest(result.Error);
            }

            var roles = _mapper.Map<IEnumerable<UserRoleDto>>(result.Data);
            return Ok(roles);
        }
    }
}
