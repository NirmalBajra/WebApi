using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly FirstRunDbContext dbContext;
        private readonly IUserServices userServices;
        public UserController(FirstRunDbContext dbContext, IUserServices userServices){
            this.dbContext = dbContext;
            this.userServices = userServices;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto dto){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            try{
                    await userServices.RegisterUser(dto);
                    return Ok ("User register Successfully");
            }
            catch(Exception e){
                return Conflict(new { message = e.Message });
            }
        }

        //Get User
        [HttpGet("users")]
        public async Task<IActionResult> ViewAllUsers(){
            var users = await userServices.ViewAllUsers();
            return Ok(users);
        }

        //View Users By Id
        [HttpGet("view/{id}")]
        public async Task<IActionResult> ViewUserById(int id){
            var user = await userServices.ViewUserById(id);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            try
            {
                await userServices.Login(dto);
                return Ok(new {message = "Login Successful."});
            }
            catch (Exception e)
            {
                return Unauthorized(new {error = e.Message});
            }
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
            {
                await userServices.Logout();
                return Ok(new {message = "Logout Successfull."});
            }
    }
}
