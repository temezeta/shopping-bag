using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace shopping_bag.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // TODO: Change all endpoints without AllowAnonymous to require authorization after it has been implemented
    public class BaseApiController : ControllerBase
    {
        /*
         * This is a base controller which all other controllers inherit.
         * In the future it should provide easy access to common operations such as getting current user
         */
    }
}
