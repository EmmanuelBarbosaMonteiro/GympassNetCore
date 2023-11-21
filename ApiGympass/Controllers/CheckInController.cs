using ApiGympass.Data.Dtos;
using ApiGympass.Services.ErrorHandling;
using ApiGympass.Services.Interfaces;
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

        [HttpPost]
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
            catch (UserNotFoundError ex)
            {
                _logger.LogWarning(ex, "User not found while creating check-in.");
                return NotFound(ex.Message);
            }
            catch (CheckInLimitExceeded ex)
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCheckInById(Guid id)
        {
            try
            {
                var checkIn = await _checkInService.GetCheckInByIdAsync(id);
                _logger.LogInformation("Check-in retrieved successfully with ID: {CheckInId}", checkIn.Id);
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
    }
}