using Application.IService;
using Application.Service;
using demo_api.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace demo_api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _user;
        public AccountController(IUserService user) {
            _user = user;


        }
        [Authorize(Roles = "Staff")]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoleByUserId()
        {
            var userIdClaim =(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!);
            int userId = Convert.ToInt32(userIdClaim.Value); 

            var result = await _user.getUserRole(userId); 

            return Ok(result); 
        }

        [HttpGet("error")]
        public async Task<IActionResult> getError()
        {
            throw new CustomException("Sample error",400);
        }
    }
}
