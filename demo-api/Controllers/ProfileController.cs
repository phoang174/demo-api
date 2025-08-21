using Application.Dtos;
using Application.IService;
using Application.Service;
using Application.Users.Commands;
using Application.Users.Queries;
using demo_api.Attributes;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace demo_api.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ISender _sender;
        public ProfileController(ISender sender) {
            _sender = sender;
        }

        [Message("Lấy danh sách user thành công")]
        [HttpGet]
        [Authorize(Roles = "Staff,Admin,SuperAdmin")]
        public async Task<IActionResult> getAllProfile( )
        {
            var result = await this._sender.Send(new GetAllUserProfilesQuery{ });
           
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Staff,Admin,SuperAdmin")]
        public async Task<IActionResult> createUserProfile(CreateUserCommand command) {

            //var result = await this._userService.CreateUserProfile(createProfileDto);
            var result = await _sender.Send(command);
            return Ok(result);


        }
        [HttpDelete("{userId}")]
        [Authorize(Roles = "Staff,Admin,SuperAdmin")]
        public async Task<IActionResult> deleteUser(int userId)
        {

            //await this._userService.DeleteUser(userId);
            return Ok();


        }
        [HttpPut]
        [Authorize(Roles = "Staff,Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateUser(UpdateUserCommand command)
        {

            var result = await this._sender.Send(command);
            return Ok(result);


        }

    }
}
