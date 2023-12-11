using ApiGympass.Data.Dtos;
using ApiGympass.Services.ErrorHandling;
using ApiGympass.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGympass.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInService _checkInService;
        private readonly ILogger<CheckInController> _logger;

        public CheckInController(ICheckInService checkInService, ILogger<CheckInController> logger)
        {
            _checkInService = checkInService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new check-in.
        /// </summary>
        /// <remarks>
        /// This method creates a new check-in with the provided details.
        /// </remarks>
        /// <param name="createCheckInDto">The check-in data transfer object containing check-in details.</param>
        /// <response code="201">If the check-in is created successfully.</response>
        /// <response code="400">If the request has invalid model state or a check-in constraint violation occurs.</response>
        /// <response code="404">If the specified gym or user is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ReadCheckInDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCheckIn([FromBody] CreateCheckInDto createCheckInDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateCheckIn request has invalid model state.");
                return BadRequest(ModelState);
            }
            try
            {
                var readCheckInDto  = await _checkInService.CreateCheckInAsync(createCheckInDto);
                _logger.LogInformation("Check-in created successfully with ID: {CheckInId}", readCheckInDto .Id);
                return new ObjectResult(readCheckInDto) { StatusCode = StatusCodes.Status201Created };
            }
            catch (GymNotFoundError ex)
            {
                _logger.LogWarning(ex, "Gym not found while creating check-in.");
                return NotFound(ex.Message);
            }
            catch (UserNotFoundError ex)
            {
                _logger.LogWarning(ex, "User not found while creating check-in.");
                return NotFound(ex.Message);
            }
            catch (CheckInDistanceViolationError ex)
            {
                _logger.LogWarning(ex, "Check-in distance violation while creating check-in.");
                return BadRequest(ex.Message);
            }
            catch (CheckInLimitExceededError ex)
            {
                _logger.LogWarning(ex, "Check-in limit exceeded while creating check-in.");
                return BadRequest(ex.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled exception occurred while creating check-in.");
                return StatusCode(500, "An internal error occurred while creating the check-in.");
            }
        }

        /// <summary>
        /// Retrieves a check-in by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the check-in.</param>
        /// <response code="200">If the check-in is found.</response>
        /// <response code="404">If no check-in is found with the specified ID.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReadCheckInDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCheckInById(Guid id)
        {
            try
            {
                var checkIn = await _checkInService.GetCheckInByIdAsync(id);
                _logger.LogInformation("Check-in retrieved successfully with ID: {CheckInId}", checkIn?.Id);
                return Ok(checkIn);
            }
            catch (CheckInNotFoundError ex)
            {
                _logger.LogWarning(ex, "Check-in not found with ID: {CheckInId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while retrieving check-in.");
                return StatusCode(500, "An error occurred while retrieving the check-in.");
            }
        }

        /// <summary>
        /// Retrieves a list of check-ins for a specific user.
        /// </summary>
        /// <remarks>
        /// This method returns check-ins for the given user ID. Access is restricted to administrators.
        /// </remarks>
        /// <param name="userId">The ID of the user whose check-ins are being retrieved.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <response code="200">If check-ins are found for the user.</response>
        /// <response code="403">If the user does not have administrator privileges.</response>
        /// <response code="404">If no user is found with the specified ID.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("CheckIns/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<ReadCheckInDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCheckInsByUserIdAsync(Guid userId, [FromQuery] int page = 1)
        {
            try
            {
                var (checkIns, hasNextPage) = await _checkInService.GetCheckInsByUserIdAsync(userId, page);

                if (hasNextPage)
                {
                    Response.Headers.Add("X-HasMorePages", "true");
                    Response.Headers.Add("X-NextPage", $"{Request.Path}?page={page + 1}");
                }
                else
                {
                    Response.Headers.Add("X-HasMorePages", "false");
                }

                _logger.LogInformation("Check-ins retrieved successfully for user with ID: {UserId}", userId);
                return Ok(checkIns);
            }
            catch (UserNotFoundError ex)
            {
                _logger.LogWarning(ex, "User not found while retrieving check-ins.");
                return NotFound(ex.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while retrieving check-ins.");
                return StatusCode(500, "An error occurred while retrieving the check-ins.");
            }
        }

        /// <summary>
        /// Validates a check-in by its ID.
        /// </summary>
        /// <remarks>
        /// This method validates a specific check-in. Access is restricted to administrators.
        /// </remarks>
        /// <param name="checkInId">The ID of the check-in to validate.</param>
        /// <response code="200">If the check-in is validated successfully.</response>
        /// <response code="403">If the user does not have administrator privileges.</response>
        /// <response code="400">If the check-in is not validated due to being late or other constraints.</response>
        /// <response code="404">If no check-in is found with the specified ID.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("Validated/{checkInId}")]
        [ProducesResponseType(typeof(ReadCheckInDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidatedCheckInById(Guid checkInId)
        {
            try
            {
                var checkIn = await _checkInService.ValidatedCheckIn(checkInId);
                _logger.LogInformation("Check-in retrieved successfully with ID: {CheckInId}", checkInId);
                return Ok(checkIn);
            }
            catch (CheckInNotFoundError ex)
            {
                _logger.LogWarning(ex, "Check-in not found with ID: {CheckInId}", checkInId);
                return NotFound(ex.Message);
            }
            catch (LateCheckInValidationError ex)
            {
                _logger.LogInformation("Attempted to confirm check-in using ID: {checkInId}, but it was delayed by more than 20 minutes.", checkInId);
                return BadRequest(ex.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while retrieving check-in.");
                return StatusCode(500, "An error occurred while retrieving the check-in.");
            }
        }
    }
}