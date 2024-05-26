using BookStore.Models;
using BookStore.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.UnitOfwork;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfwork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthController(IUnitOfwork unitOfWork, IJwtService jwtService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = _unitOfWork.UserRepo.Get()
                               .FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user);

            _jwtService.SaveToken(user, token, DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"])));
           

            return Ok(new { Token = token, RefreshToken = refreshToken });
        }


        [HttpPost("logout")]
        public IActionResult Logout([FromBody] RevokeTokenModel model)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(model.Token);
            if (principal == null)
            {
                return BadRequest("Invalid token");
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID claim");
            }

            var tokens = _unitOfWork.TokenRepo.Get()
                                .Where(t => t.UserId == userId && t.IsActive).ToList();

            foreach (var token in tokens)
            {
                token.IsActive = false;
                _unitOfWork.TokenRepo.Update(token);
            }

            _unitOfWork.Save();
            return NoContent();
        }


    }
}

