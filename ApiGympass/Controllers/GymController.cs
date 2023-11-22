using ApiGympass.Data.Dtos;
using ApiGympass.Services.ErrorHandling;
using ApiGympass.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiGympass.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GymController : ControllerBase
    {
        private readonly IGymService _gymService;
        private readonly ILogger<GymController> _logger;

        public GymController(IGymService gymService, ILogger<GymController> logger)
        {
            _gymService = gymService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGym([FromBody] CreateGymDto gymDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateGym request has invalid model state.");
                return BadRequest(ModelState);
            }
            try
            {
                var createdGym = await _gymService.CreateGymAsync(gymDto);
                _logger.LogInformation("Gym created successfully with ID: {GymId}", createdGym.Id);
                return new ObjectResult(createdGym) { StatusCode = StatusCodes.Status201Created };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while creating gym.");
                return StatusCode(500, "An error occurred while creating the gym.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGymById(Guid id)
        {
            try
            {
                var gym = await _gymService.FindById(id);
                return Ok(gym);
            }
            catch (GymNotFoundError)
            {
                _logger.LogWarning("No gym found with ID: {id}", id);
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while retrieving gym with ID: {id}", id);
                return StatusCode(500, "An error occurred while retrieving the gym.");
            }
        }
    }
}