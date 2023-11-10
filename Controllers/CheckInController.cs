using ApiGympass.Data.Dtos;
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
        public async Task<IActionResult> CreateCheckIn(CreateCheckInDto checkInDto)
        {
            _logger.LogInformation("Received request to create check-in.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateCheckIn request has invalid model state.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var checkIn = await _checkInService.CreateCheckInAsync(checkInDto);
                if (checkIn == null)
                {
                    _logger.LogError("Failed to create check-in due to null response from service.");
                    return BadRequest("Failed to create check-in.");
                }

                _logger.LogInformation("Check-in created successfully with ID: {CheckInId}", checkIn.Id);
                return CreatedAtAction(nameof(GetCheckInById), new { id = checkIn.Id }, checkIn);
            }
            catch (ArgumentException e)
            {
                _logger.LogWarning(e, "Exception occurred while creating check-in.");
                return StatusCode(500, "An error occurred while creating the check-in.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCheckInById(Guid id)
        {
            _logger.LogInformation("Received request to get check-in with ID: {id}", id);

            try
            {
                var checkIn = await _checkInService.GetCheckInByIdAsync(id);
                if (checkIn == null)
                {
                    _logger.LogWarning("No check-in found with ID: {id}", id);
                    return NotFound();
                }

                _logger.LogInformation("Check-in retrieved successfully with ID: {CheckInId}", checkIn.Id);
                return Ok(checkIn);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while retrieving check-in.");
                return StatusCode(500, "An error occurred while retrieving the check-in.");
            }
        }
    }
}