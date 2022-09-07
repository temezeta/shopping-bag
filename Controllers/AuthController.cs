using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shopping_bag.DTOs.User;
using shopping_bag.Models.User;
using shopping_bag.Services;
using shopping_bag.Utility;

namespace shopping_bag.Controllers
{
    public class AuthController : BaseApiController
    {

        public AuthController(IUserService userService) : base(userService)
        {
        }

        [HttpPost, AllowAnonymous]
        [Route("register")]
        public async Task<ActionResult> Register([FromBody]RegisterDto request)
        {
            if(!AuthHelper.ValidatePassword(request.Password, request.RepeatPassword))
            {
                return BadRequest();
            }

            AuthHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var newUser = new NewUser {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            var success = await _userService.AddUser(newUser);

            if (success)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost, AllowAnonymous]
        [Route("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto request)
        {
            var user = await _userService.GetUserByEmail(request.Email);

            if(user == null)
            {
                return BadRequest();
            }

            if(!AuthHelper.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest();
            }

            var token = AuthHelper.CreateToken(user);
            var refreshToken = AuthHelper.GenerateRefreshToken();
            await SetRefreshToken(user, refreshToken);

            return Ok(new TokenResponseDto { Token = token });
        }

        [HttpPost, AllowAnonymous]
        [Route("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody]RefreshTokenDto request)
        {
            var email = AuthHelper.GetEmailFromExpiredToken(request.ExpiredToken);
            var user = email != null ? await _userService.GetUserByEmail(email) : null;
            var refreshToken = Request.Cookies["refreshToken"];

            if(refreshToken == null || user == null)
            {
                return BadRequest();
            } 
            else if (!user.RefreshToken.Equals(refreshToken) || user.TokenExpiresAt < DateTime.Now)
            {
                return Unauthorized();
            }

            string token = AuthHelper.CreateToken(user);
            var newRefreshToken = AuthHelper.GenerateRefreshToken();
            await SetRefreshToken(user, newRefreshToken);

            return Ok(new TokenResponseDto { Token = token });
        }

        private async Task SetRefreshToken(User user, RefreshToken refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.ExpiresAt
            };

            user.RefreshToken = refreshToken.Token;
            user.TokenCreatedAt = refreshToken.CreatedAt;
            user.TokenExpiresAt = refreshToken.ExpiresAt;
            await _userService.SetRefreshToken(user);

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }
    }
}
