﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shopping_bag.DTOs.User;
using shopping_bag.Models.Email;
using shopping_bag.Models.User;
using shopping_bag.Services;
using shopping_bag.Utility;

namespace shopping_bag.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        public AuthController(IUserService userService, IAuthService authService, IEmailService emailService) : base(userService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        [HttpPost, AllowAnonymous]
        [Route("register")]
        public async Task<ActionResult> Register([FromBody]RegisterDto request)
        {
            var response = await _authService.Register(request);

            if (!response.IsSuccess)
            {
                return BadRequest();
            }

            // TODO Make verification email more nice
            var emailResponse = _emailService.SendEmail(new Email
            {
                To = request.Email,
                Subject = "Email Verification",
                Body = response.Data
            });

            if (!emailResponse.IsSuccess)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost, AllowAnonymous]
        [Route("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto request)
        {
            var response = await _authService.Login(request);

            if(!response.IsSuccess)
            {
                return BadRequest();
            }

            var refreshToken = AuthHelper.GenerateRefreshToken();
            await SetRefreshToken(response.Data.User, refreshToken);

            return Ok(new TokenResponseDto { Token = response.Data.LoginToken });
        }

        [HttpPost, AllowAnonymous]
        [Route("verify")]
        public async Task<ActionResult> Verify([FromBody] string verificationToken)
        {
            var response = await _authService.VerifyUserToken(verificationToken);

            if (!response.IsSuccess)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost, AllowAnonymous]
        [Route("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody]RefreshTokenDto request)
        {
            var email = AuthHelper.GetEmailFromExpiredToken(request.ExpiredToken);

            if(email == null)
            {
                return BadRequest();
            }

            var response = await _userService.GetUserByEmail(email);
            var refreshToken = Request.Cookies["refreshToken"];

            if(!response.IsSuccess || response.Data.RefreshToken == null)
            {
                return BadRequest();
            } 
            else if (!response.Data.RefreshToken.Equals(refreshToken) || response.Data.TokenExpiresAt < DateTime.Now)
            {
                return Unauthorized();
            }

            string token = AuthHelper.CreateToken(response.Data);
            var newRefreshToken = AuthHelper.GenerateRefreshToken();
            await SetRefreshToken(response.Data, newRefreshToken);

            return Ok(new TokenResponseDto { Token = token });
        }

        [HttpPost, AllowAnonymous]
        [Route("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] string email)
        {
            var response = await _authService.SetPasswordResetToken(email);

            if (!response.IsSuccess)
            {
                return BadRequest();
            }

            // TODO Make password email more nice
            var emailResponse = _emailService.SendEmail(new Email
            {
                To = email,
                Subject = "Password Reset",
                Body = response.Data
            });

            if (!emailResponse.IsSuccess)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost, AllowAnonymous]
        [Route("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var response = await _authService.ResetPassword(request);

            if (!response.IsSuccess)
            {
                return BadRequest();
            }

            return Ok();
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
            await _authService.SetRefreshToken(user);

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }
    }
}