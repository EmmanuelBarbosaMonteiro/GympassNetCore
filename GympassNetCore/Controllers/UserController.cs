using ApiGympass.Data.Dtos;
using ApiGympass.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using ApiGympass.Services.ErrorHandling;
using Project.Services.ErrorHandling;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ApiGympass.Services.Implementations;
using GympassNetCore.Utils;

namespace ApiGympass.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly TokenService _tokenService;

        public UserController(IUserService userService, ILogger<UserController> logger, TokenService tokenService)
        {
            _userService = userService;
            _logger = logger;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <remarks>
        /// This method creates a new user with the given details.
        /// </remarks>
        /// <param name="dto">The user data transfer object containing user details.</param>
        /// <response code="201">If the user is created successfully. The response includes the user details.</response>
        /// <response code="400">If the request is invalid, typically due to invalid model state.</response>
        /// <response code="409">If a user with the same identifier already exists.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ReadUserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for CreateUser");
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.CreateUserAsync(dto);
                _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);
                return new ObjectResult(user) { StatusCode = StatusCodes.Status201Created };
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

        /// <summary>
        /// Authenticates a user and provides an access token.
        /// </summary>
        /// <remarks>
        /// This method authenticates the user with the provided login credentials. 
        /// If successful, it returns an access token and sets a refresh token in an HTTP-only cookie.
        /// </remarks>
        /// <param name="dto">The login data transfer object containing user credentials.</param>
        /// <response code="200">If the user is logged in successfully. The response includes the access token.</response>
        /// <response code="400">If the request is invalid, typically due to an invalid model state.</response>
        /// <response code="401">If the login credentials are invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for LoginUser");
                return BadRequest(ModelState);
            }
            try
            {
                var user = await _userService.GetUserByEmailAsync(dto.Email ?? string.Empty);
                await _userService.LoginUserAsync(dto);

                var accessToken = _tokenService.GenerateToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken(user);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                };
                Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);

                _logger.LogInformation("User logged in successfully");
                return Ok(new { accessToken });
            }
            catch (InvalidCredentialsError ex)
            {
                _logger.LogWarning(ex, "Invalid credentials");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception in LoginUser");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// Refreshes the access token using a refresh token stored in a cookie.
        /// </summary>
        /// <remarks>
        /// This method validates the refresh token and issues a new access token if the refresh token is valid.
        /// </remarks>
        /// <response code="200">If the refresh token is valid and a new access token is successfully issued.</response>
        /// <response code="400">If there is no refresh token provided or it is invalid.</response>
        /// <response code="401">If the refresh token is unauthorized or has expired.</response>
        /// <response code="500">If an internal server error occurs during the process.</response>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("Invalid refresh token.");
            }

            ClaimsPrincipal principal;
            try
            {
                principal = _tokenService.GetPrincipalFromExpiredToken(refreshToken);
            }
            catch
            {
                return Unauthorized("Invalid refresh token.");
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid refresh token.");
            }

            var userDto = await _userService.GetByIdAsync(new Guid(userId));
            if (userDto == null)
            {
                return BadRequest();
            }
            var user = await _userService.GetUserByEmailAsync(userDto.Email);

            if (user == null)
            {
                return Unauthorized();
            }

            var newAccessToken = _tokenService.GenerateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Set to false if testing locally without HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("RefreshToken", newRefreshToken, cookieOptions);

            return Ok(new { accessToken = newAccessToken });
        }

        /// <summary>
        /// Updates the details of an existing user.
        /// </summary>
        /// <remarks>
        /// This method updates a user's profile based on the provided information. 
        /// Users are only allowed to update their own profiles.
        /// </remarks>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="updateUserDto">The user data transfer object containing updated user details.</param>
        /// <response code="200">If the user is updated successfully.</response>
        /// <response code="400">If the request is invalid, typically due to an invalid model state.</response>
        /// <response code="401">If the user is unauthorized to update the specified user's profile.</response>
        /// <response code="404">If the user with the specified ID is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateUser: Invalid model");
                return BadRequest(ModelState);
            }
            try
            {
                var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId.ToString() != authenticatedUserId)
                {
                    return Unauthorized("Access denied. You can only access your own profile.");
                }
                
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

        /// <summary>
        /// Partially updates the details of an existing user.
        /// </summary>
        /// <remarks>
        /// This method applies a JSON patch to a user's profile. 
        /// Users are only allowed to patch their own profiles.
        /// </remarks>
        /// <param name="userId">The ID of the user to patch.</param>
        /// <param name="patchDoc">The JSON patch document with updates to the user profile.</param>
        /// <response code="200">If the user is patched successfully.</response>
        /// <response code="400">If the request is invalid, typically due to an invalid model state or null patch document.</response>
        /// <response code="401">If the user is unauthorized to patch the specified user's profile.</response>
        /// <response code="404">If the user with the specified ID is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [HttpPatch("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchUser(string userId, [FromBody] JsonPatchDocument<UpdateUserDto> patchDoc)
        {
            if (patchDoc == null || !ModelState.IsValid)
            {
                _logger.LogWarning("PatchUser: Invalid model");
                return BadRequest(ModelState);
            }
            try
            {
                var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId.ToString() != authenticatedUserId)
                {
                    return Unauthorized("Access denied. You can only access your own profile.");
                }

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

        /// <summary>
        /// Retrieves a user's profile based on their user ID.
        /// </summary>
        /// <remarks>
        /// This method returns the profile of a user. Users can only access their own profiles.
        /// </remarks>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <response code="200">If the user is found and returned successfully.</response>
        /// <response code="401">If the user is unauthorized or tries to access a profile that is not their own.</response>
        /// <response code="404">If no user is found with the specified ID.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(ReadUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId.ToString() != authenticatedUserId)
                {
                    return Unauthorized("Access denied. You can only access your own profile.");
                }

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

        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        /// <remarks>
        /// This method returns a list of all users. Access is restricted to administrators.
        /// </remarks>
        /// <response code="200">Returns the list of all users.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have administrator privileges.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReadUserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers()
        {
            var userDtos = await _userService.GetAllUsersAsync();
            _logger.LogInformation("Retrieved all users successfully.");
            return Ok(userDtos);
        }

        /// <summary>
        /// Deletes a specified user.
        /// </summary>
        /// <remarks>
        /// This method deletes a user based on the provided user ID. Access is restricted to administrators.
        /// </remarks>
        /// <param name="userId">The ID of the user to be deleted.</param>
        /// <response code="200">If the user is deleted successfully.</response>
        /// <response code="404">If no user is found with the specified ID.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have administrator privileges.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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