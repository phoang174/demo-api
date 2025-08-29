using Application.Dtos;
using Application.IService;
using Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace demo_api.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ISender _sender;
        public RoleController(ISender sender)
        {
            _sender = sender;
        }
        [HttpGet]
        [Authorize(Roles = "Staff,Admin,SuperAdmin")]
        public async Task<IActionResult> GetAllRoles()
        {

            var result = await this._sender.Send(new GetUserRolesQuery { });
            return Ok(result.Value);


        }
    }
}
