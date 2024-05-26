using Repository.Models;
using System.Security.Claims;

namespace BookStore.Service
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken(User user);
        public void SaveToken(User user, string tokenString, DateTime expiryDate);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
