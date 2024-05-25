using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.UnitOfwork;

namespace BookStore.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IUnitOfwork unitOfwork;

        public BookController(IUnitOfwork unitOfwork)
        {
            this.unitOfwork = unitOfwork;
        }
        [HttpGet]
        public IActionResult GetBook()
        {
            return Ok(unitOfwork.BookRepo.Get());
        }
    }
}
