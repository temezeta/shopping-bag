using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using shopping_bag.Config;
using shopping_bag.Models.Email;
using shopping_bag.Models.User;
using shopping_bag.Services;
using System.Security.Claims;

namespace shopping_bag.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        protected readonly IUserService _userService;
        protected BaseApiController(IUserService userService)
        {
            _userService = userService;
        }

        protected async Task<User?> GetCurrentUser()
        {
            var email = User?.FindFirst(ClaimTypes.Email);

            if (email == null)
            {
                return null;
            }

            var response = await _userService.GetUserByEmail(email.Value);

            if (!response.IsSuccess)
            {
                return null;
            }
            return response.Data;
        }

        protected string GetVerificationBodyText(string verificationToken)
        {
            // Generate verification url
            var verificationUrl = Url.ActionLink("Verify", "Auth", new { verificationToken = verificationToken });
            
            verificationUrl = "<a href=\"" + verificationUrl + "\">VERIFY ACCOUNT</a>.";
            var verificationBodyText = string.Format(StaticConfig.VerificationEmailBodyText, verificationUrl);

            return verificationBodyText;
        }
    }
}
