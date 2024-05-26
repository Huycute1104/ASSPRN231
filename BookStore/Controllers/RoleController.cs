using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.UnitOfwork;

namespace BookStore.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IUnitOfwork unitOfwork;

        public RoleController(IUnitOfwork unitOfwork)
        {
            this.unitOfwork = unitOfwork;
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetBook()
        {
            return Ok(unitOfwork.RoleRepo.Get());
        }
    }
}
