using Microsoft.IdentityModel.Tokens;
using shopping_bag.Config;
using shopping_bag.Models.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace shopping_bag.Utility
{
    public static class AuthHelper
    {
        public static string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            user.UserRoles.ForEach(it => claims.Add(new Claim(ClaimTypes.Role, it.Role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(StaticConfig.Token));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(StaticConfig.TokenValidityInMinutes),
                signingCredentials: cred,
                issuer: StaticConfig.Issuer,
                audience: StaticConfig.Audience);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        
        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }
        
        public static bool ValidatePassword(string password, string repeatPassword)
        {
            // TODO: Somekind of password validation here, also validate email somewhere
            return true;
        }
        
        public static RefreshToken GenerateRefreshToken()
        {
            var bytes = new byte[64];
            RandomNumberGenerator.Create().GetBytes(bytes);
            var refreshToken = new RefreshToken
            {
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(StaticConfig.RefreshTokenValidityDays),
                Token = Convert.ToBase64String(bytes),
            };

            return refreshToken;
        }
        
        public static string GetEmailFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(StaticConfig.Token)),
                    ValidateLifetime = false,
                    ValidIssuer = StaticConfig.Issuer,
                    ValidAudience = StaticConfig.Audience
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal?.FindFirst(ClaimTypes.Email)?.Value;
            } 
            catch
            {
                return null;
            }
        }

        public static string CreateHexToken()
        {
            var bytes = new byte[32];
            RandomNumberGenerator.Create().GetBytes(bytes);
            return Convert.ToHexString(bytes);
        }
    }
}
