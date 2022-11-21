using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shopping_bag.DTOs.Office;
using shopping_bag.Services;

namespace shopping_bag.Controllers {

    public class OfficeController : BaseApiController {

        private readonly IOfficeService _officeService;
        private readonly IMapper _mapper;
        public OfficeController(IUserService userService, IOfficeService officeService, IMapper mapper) : base(userService) {
            _officeService = officeService ?? throw new ArgumentNullException(nameof(officeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get/all")]
        public async Task<ActionResult<IEnumerable<OfficeDto>>> GetAllOffices() {
            var result = await _officeService.GetOffices();
            if(!result.IsSuccess) {
                return BadRequest(result.Error);
            }
            return Ok(_mapper.Map<IEnumerable<OfficeDto>>(result.Data));
        }

        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OfficeDto>> AddOffice([FromBody] AddOfficeDto office) {
            if (office == null || string.IsNullOrEmpty(office.Name)) {
                return BadRequest("Office name is required");
            }
            var result = await _officeService.AddOffice(office);
            if(!result.IsSuccess) {
                return BadRequest(result.Error);
            }
            return _mapper.Map<OfficeDto>(result.Data);
        }

        [HttpPut]
        [Route("")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OfficeDto>> ModifyOffice([FromBody] AddOfficeDto office, long officeId)
        {
            if (office == null || string.IsNullOrEmpty(office.Name))
            {
                return BadRequest("Office name is required");
            }

            var response = await _officeService.ModifyOffice(office, officeId);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Error);
            }

            return _mapper.Map<OfficeDto>(response.Data);
        }
    }
}
