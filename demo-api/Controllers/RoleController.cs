using Application.Dtos;
using Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace demo_api.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpGet]
        [Authorize(Roles = "Staff,Admin,SuperAdmin")]
        public async Task<IActionResult> GetAllRoles()
        {

            var result = await this._roleService.getAllRoles();
            return Ok(result);


        }
    }
}
