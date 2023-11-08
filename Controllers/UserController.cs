using ApiGympass.Data.Dtos;
using ApiGympass.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiGympass.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
         private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> UserRegister([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.CreateUserAsync(dto);

            if (result.Succeeded)
            {
                return Ok("User created successfully");
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description);

                return BadRequest(errors);
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateUserAsync(userId, updateUserDto);

            if (result.Succeeded)
            {
                return Ok("User updated successfully");
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description);

                return BadRequest(errors);
            }
        }

        [HttpPatch("{userId}")]
        public async Task<IActionResult> PatchUser(string userId, [FromBody] JsonPatchDocument<UpdateUserDto> patchDoc)
        {
            if (patchDoc == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.PatchUserAsync(userId, patchDoc);

            if (result.Succeeded)
            {
                return Ok("User patched successfully");
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description);

                return BadRequest(errors);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var userDto = await _userService.GetByIdAsync(userId);

            if (userDto == null)
            {
                return NotFound($"User with ID {userId} was not found.");
            }

            return Ok(userDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var userDtos = await _userService.GetAllUsersAsync();

            return Ok(userDtos);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            if (result.Succeeded)
            {
                return Ok("User deleted successfully.");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}