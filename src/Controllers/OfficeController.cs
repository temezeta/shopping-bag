using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shopping_bag.DTOs.Office;
using shopping_bag.Services;

namespace shopping_bag.Controllers {

    public class OfficeController : BaseApiController {

        private readonly IOfficeService _officeService;
        public OfficeController(IUserService userService, IOfficeService officeService) : base(userService) {
            _officeService = officeService ?? throw new ArgumentNullException(nameof(officeService));
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get/all")]
        public async Task<ActionResult<IEnumerable<OfficeDto>>> GetAllOffices() {
            var result = await _officeService.GetOffices();
            if(!result.IsSuccess) {
                return BadRequest(result.Error);
            }
            return Ok(result.Data.Select(o => new OfficeDto() {
                Id = o.Id,
                Name = o.Name
            }).ToList());
        }

        [HttpPost]
        [AllowAnonymous]
        // TODO: Remove AllowAnonymous, now only for adding office as you can't register before that
        [Route("add")]
        public async Task<ActionResult<OfficeDto>> AddOffice([FromBody] AddOfficeDto office) {
            if (office == null || string.IsNullOrEmpty(office.Name)) {
                return BadRequest("Office name is required");
            }
            var result = await _officeService.AddOffice(office);
            if(!result.IsSuccess) {
                return BadRequest(result.Error);
            }
            return Ok(new OfficeDto() { 
                Id = result.Data.Id,
                Name = result.Data.Name
            });
        }  
    }
}
