using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shopping_bag.DTOs.Office;
using shopping_bag.DTOs.User;
using shopping_bag.Models;
using shopping_bag.Models.User;
using shopping_bag.Services;

namespace shopping_bag.Controllers {
    public class UserController : BaseApiController {

        public UserController(IUserService userService) : base(userService) {
        }

        [HttpGet]
        [Route("list")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUserList() {
            var result = await _userService.GetUsers();
            if(!result.IsSuccess) {
                return BadRequest(result.Error);
            }

            var users = result.Data.Select(u => MapUserToDto(u)).ToList();
            return users;
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<ActionResult<UserDto>> GetUserById([FromRoute] long userId) {
            var result = await _userService.GetUserById(userId);
            if (!result.IsSuccess) {
                return BadRequest(result.Error);
            }

            var user = MapUserToDto(result.Data);
            return user;
        }

        // TODO: Create public mapping methods somewhere if same mappings are also used elsewhere.
        // Maybe mapper classes for relevant model+dto pairs. Dependency injected like services are right now.
        private UserDto MapUserToDto(User user) {
            return new UserDto() {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                HomeOffice = MapOfficeToDto(user.HomeOffice),
                OfficeId = user.OfficeId,
                UserRoles = user.UserRoles.Select(r => MapUserRoleToDto(r))
            };
        }
        private OfficeDto MapOfficeToDto(Office office) {
            return new OfficeDto() { 
                Id = office.Id, 
                Name = office.Name 
            };
        }
        private UserRoleDto MapUserRoleToDto(UserRole userRole) {
            return new UserRoleDto() {
                RoleId = userRole.RoleId,
                RoleName = userRole.RoleName
            };
        }
    }
}
