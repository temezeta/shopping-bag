﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using shopping_bag.Config;
using shopping_bag.DTOs.User;
using shopping_bag.Models;
using shopping_bag.Models.Email;
using shopping_bag.Models.User;
using shopping_bag.Utility;

namespace shopping_bag.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        public AuthService(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<ServiceResponse<string>> Register(RegisterDto request)
        {
            var response = await _userService.GetUserByEmail(request.Email);
            if (response.IsSuccess)
            {
                return new ServiceResponse<string>(error: "User with email already exists");
            }

            var getUser = await _userService.IsUserRemoved(request.Email);

            AuthHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var verificationToken = AuthHelper.CreateHexToken();

            var officeExists = _context.Offices.Any(office => office.Id == request.OfficeId);
    
            if (!officeExists)
            {
                return new ServiceResponse<string>(error: "Office doesn't exist");
            }

            var defaultRole = _context.UserRoles.FirstOrDefault(r => r.RoleName.Equals(Roles.DefaultRole));
            if (defaultRole == null) {
                // TODO: Shouldn't happen ever because roles should always exist.
                // Throw exception or something instead?
                return new ServiceResponse<string>(error: "User role doesn't exist in database.");
            }

            if (getUser.Data != null)
            {
                var userData = getUser.Data;
                userData.FirstName = request.FirstName;
                userData.LastName = request.LastName;
                userData.Email = userData.Email;
                userData.OfficeId = request.OfficeId;
                userData.PasswordHash = passwordHash;
                userData.PasswordSalt = passwordSalt;
                userData.VerificationToken = verificationToken;
                userData.VerifiedAt = null;
                userData.RefreshToken = null;
                userData.TokenCreatedAt = null;
                userData.TokenExpiresAt = null;
                userData.PasswordResetToken = null;
                userData.ResetTokenExpires = null;
                userData.Removed = false;

                await _context.SaveChangesAsync();
            } 
            else
            {
                try
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        var user = new User
                        {
                            FirstName = request.FirstName,
                            LastName = request.LastName,
                            Email = request.Email,
                            OfficeId = request.OfficeId,
                            PasswordHash = passwordHash,
                            PasswordSalt = passwordSalt,
                            VerificationToken = verificationToken
                        };
                        user.UserRoles.Add(defaultRole);
                        _context.Users.Add(user);
                        _context.SaveChanges();
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    return new ServiceResponse<string>(error: ex.Message);
                }
            }

            return new ServiceResponse<string>(data: verificationToken);
        }

        public async Task<ServiceResponse<LoginData>> Login(LoginDto request)
        {
            var response = await _userService.GetUserByEmail(request.Email);

            if (!response.IsSuccess)
            {
                return new ServiceResponse<LoginData>(error: response.Error);
            }
            else if (response.Data.VerifiedAt == null){
                return new ServiceResponse<LoginData>(error: "User not verified");
            }
            else if(!AuthHelper.VerifyPasswordHash(request.Password, response.Data.PasswordHash, response.Data.PasswordSalt))
            {
                return new ServiceResponse<LoginData>(error: "Invalid password");
            }

            return new ServiceResponse<LoginData>(data: new LoginData
            {
                User = response.Data,
                LoginToken = AuthHelper.CreateToken(response.Data)
            });
        }

        public async Task<ServiceResponse<bool>> Logout(User user)
        {
            var found = await _context.Users.FindAsync(user.Id);

            if (found == null)
            {
                return new ServiceResponse<bool>(error: "User not found");
            }

            found.RefreshToken = null;
            found.TokenCreatedAt = null;
            found.TokenExpiresAt = null;
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>(data: true);
        }

        public async Task<ServiceResponse<bool>> SetRefreshToken(User user)
        {
            var found = await _context.Users.FindAsync(user.Id);

            if (found == null)
            {
                return new ServiceResponse<bool>(error: "User not found");
            }

            found.RefreshToken = user.RefreshToken;
            found.TokenCreatedAt = user.TokenCreatedAt;
            found.TokenExpiresAt = user.TokenExpiresAt;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(data: true);
        }

        public async Task<ServiceResponse<bool>> VerifyUserToken(string verificationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == verificationToken);

            if(user == null)
            {
                return new ServiceResponse<bool>(error: "User not found");
            }

            user.VerifiedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(data: true);
        }

        public async Task<ServiceResponse<string>> SetPasswordResetToken(string email)
        {
            var resetToken = AuthHelper.CreateHexToken();
            var response = await _userService.GetUserByEmail(email);

            if (!response.IsSuccess)
            {
                return new ServiceResponse<string>(error: response.Error);
            }

            response.Data.PasswordResetToken = resetToken;
            response.Data.ResetTokenExpires = DateTime.Now.AddHours(2);
            await _context.SaveChangesAsync();
            return new ServiceResponse<string>(data: resetToken);
        }
        
        public async Task<ServiceResponse<bool>> ResetPassword(ResetPasswordDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.ResetToken);

            if(user == null)
            {
                return new ServiceResponse<bool>(error: "User not found");
            }
            else if (user.ResetTokenExpires < DateTime.Now)
            {
                return new ServiceResponse<bool>(error: "Reset token expired");
            }

            AuthHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(data: true);
        }
    }
}
