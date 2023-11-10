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
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateUser: Invalid model");
                return BadRequest(ModelState);
            }

            try
            {
                var (result, user) = await _userService.CreateUserAsync(dto);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);
                    return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, user); 
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning($"CreateUser: Failed to create user - {errors}");
                    return BadRequest(new { Error = errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateUser: Unexpected exception");
                return StatusCode(500, "An internal server error occurred.");
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
            try
            {
                var user = await _userService.GetByIdAsync(userId);

                if (user != null)
                {
                    return Ok(user);
                }
                else
                {
                    _logger.LogWarning($"GetUser: User with ID {userId} not found");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetUser: Exception while fetching user with ID {userId}");
                return StatusCode(500, "An internal server error occurred.");
            }
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