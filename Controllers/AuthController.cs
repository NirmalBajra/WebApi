using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet("forbidden")]
        public IActionResult Forbidden()
        {
            return StatusCode(StatusCodes.Status403Forbidden, "You must be 18 or older to access this.");
        }
    }
}
