using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.UnitOfwork;

namespace BookStore.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfwork unitOfwork;

        public UserController(IUnitOfwork unitOfwork)
        {
            this.unitOfwork = unitOfwork;
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetBook()
        {
            return Ok(unitOfwork.UserRepo.Get());
        }
    }
}
