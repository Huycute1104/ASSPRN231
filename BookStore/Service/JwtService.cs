using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Models;
using Repository.UnitOfwork;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BookStore.Service
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfwork _unitOfWork;

        public JwtService(IConfiguration configuration, IUnitOfwork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };
            List<Role> roles = _unitOfWork.RoleRepo.Get().ToList();
            for (int i = 0; i < roles.Count; i++)
            {
                Role role = roles[i];
                if (user.RoleId == role.RoleId)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                    break;
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            SaveToken(user, tokenString, DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"])));

            return tokenString;
        }


        public string GenerateRefreshToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("IsRefreshToken", "true")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var refreshToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpiryDays"])),
                signingCredentials: creds
            );

            var refreshTokenString = new JwtSecurityTokenHandler().WriteToken(refreshToken);
            SaveToken(user, refreshTokenString, DateTime.Now.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpiryDays"])));

            return refreshTokenString;
        }

        public void SaveToken(User user, string tokenString, DateTime expiryDate)
        {
            var lasttoken = _unitOfWork.TokenRepo.Get().LastOrDefault();
            if (lasttoken == null)
            {
                lasttoken.TokenId = 0;
            }
            var token = new Token
            {
                TokenId = lasttoken.TokenId + 1,
                UserId = user.UserId,
                Token1 = tokenString,
                ExpiryDate = expiryDate,
                IsActive = true
            };

            _unitOfWork.TokenRepo.Add(token);
            _unitOfWork.Save();
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
