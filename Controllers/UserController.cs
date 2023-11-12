using ApiGympass.Data.Dtos;
using ApiGympass.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using ApiGympass.Services.ErrorHandling;

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
                _logger.LogWarning("Invalid model state for CreateUser");
                return BadRequest(ModelState);
            }

            try
            {
                var (result, user) = await _userService.CreateUserAsync(dto);
                _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);
                return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, user);
            }
            catch (UserAlreadyExistsError ex)
            {
                _logger.LogWarning(ex, "User already exists");
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception in CreateUser");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateUser: Invalid model");
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _userService.UpdateUserAsync(userId, updateUserDto);
                _logger.LogInformation("User updated successfully with ID: {UserId}", userId);
                return Ok("User updated successfully");
            }
            catch (UserNotFoundError ex)
            {
                _logger.LogWarning(ex, $"UpdateUser: User with ID {userId} not found");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateUser: Unexpected exception");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPatch("{userId}")]
        public async Task<IActionResult> PatchUser(string userId, [FromBody] JsonPatchDocument<UpdateUserDto> patchDoc)
        {
            if (patchDoc == null || !ModelState.IsValid)
            {
                _logger.LogWarning("PatchUser: Invalid model");
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _userService.PatchUserAsync(userId, patchDoc);
                _logger.LogInformation("User patched successfully with ID: {UserId}", userId);
                return Ok("User patched successfully");
            }
            catch (UserNotFoundError ex)
            {
                _logger.LogWarning($"GetUser: User with ID {userId} not found");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PatchUser: Unexpected exception");
                return StatusCode(500, "An internal server error occurred.");
            }
            
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                var user = await _userService.GetByIdAsync(userId);
                _logger.LogInformation("User retrieved successfully with ID: {UserId}", userId);
                return Ok(user);
            }
            catch (UserNotFoundError ex)
            {
                _logger.LogWarning(ex, $"GetUser: User with ID {userId} not found");
                return NotFound(ex.Message);
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

            _logger.LogInformation("Retrieved all users successfully.");
            return Ok(userDtos);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(userId);
                _logger.LogInformation("User deleted successfully with ID: {UserId}", userId);
                return Ok("User deleted successfully.");
            }
            catch (UserNotFoundError ex)
            {
                _logger.LogWarning($"GetUser: User with ID {userId} not found");
                return NotFound(ex.Message);
            }
        }
    }
}