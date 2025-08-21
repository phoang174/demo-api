using Application.Dtos;
using Application.IService;
using Application.Service;
using demo_api.Attributes;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace demo_api.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        public ProfileController(IUserService userService) {
            _userService = userService;
        }

        [Message("Lấy danh sách user thành công")]
        [HttpGet]
        [Authorize(Roles = "Staff,Admin,SuperAdmin")]
        public async Task<IActionResult> getAllProfile()
        {
            var result = await this._userService.GetAllUserProfiles();
            foreach(var temp in result){
                Console.WriteLine(temp.UserId);

            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Staff,Admin,SuperAdmin")]
        public async Task<IActionResult> createUserProfile(CreateProfileDto createProfileDto) {

            var result = await this._userService.CreateUserProfile(createProfileDto);
            return Ok(result);


        }
        [HttpDelete("{userId}")]
        [Authorize(Roles = "Staff,Admin,SuperAdmin")]
        public async Task<IActionResult> deleteUser(int userId)
        {

            await this._userService.DeleteUser(userId);
            return Ok();


        }
        [HttpPut]
        [Authorize(Roles = "Staff,Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto createProfileDto)
        {

            var result = await this._userService.UpdateUser(createProfileDto);
            return Ok(result);


        }

    }
}
