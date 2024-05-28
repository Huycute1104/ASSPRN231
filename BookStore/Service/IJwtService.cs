using Repository.Models;
using System.Security.Claims;

namespace BookStore.Service
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken(User user);
        public void SaveToken(User user, string tokenString, DateTime expiryDate);
        public void Logout(string tokenString);
        public bool IsTokenValid(string tokenString);
    /*    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);*/
    }
}
