using BookStore.Models;
using BookStore.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
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
                return Unauthorized(new { message = "Invalid email or password" });
            }
            if (user.UserStatus == false)
            {
                return Unauthorized(new { message = "User has been banned" });
            }

            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user);     
            return Ok(
                new 
                {
                    UserInfo = new
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        Phone = user.Phone,
                        Address = user.Address,
                        UserStatus = user.UserStatus,
                        RoleId = user.RoleId,
                    },
                    Token = token, 
                    RefreshToken = refreshToken
                    
                });
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var tokenString = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(tokenString))
            {
                return BadRequest("Invalid token");
            }

            var token = _unitOfWork.TokenRepo.Get().FirstOrDefault(t => t.Token1 == tokenString);

            if (token == null)
            {
                return BadRequest("Token not found");
            }

            // Cập nhật trạng thái IsActive của token
            token.IsActive = false;
            _unitOfWork.TokenRepo.Update(token);
            _unitOfWork.Save();

            return Ok("Logged out successfully");
        }
    }




}

