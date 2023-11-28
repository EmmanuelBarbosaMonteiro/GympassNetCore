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
            catch (GymNotFoundError ex)
            {
                _logger.LogWarning("No gym found with ID: {id}", id);
                return NotFound( ex.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while retrieving gym with ID: {id}", id);
                return StatusCode(500, "An error occurred while retrieving the gym.");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchGyms([FromQuery] string query, [FromQuery] int page = 1)
        {
            try
            {
                var (gyms, hasNextPage) = await _gymService.SearchGymsAsync(query, page);

                if (hasNextPage)
                {
                    Response.Headers.Add("X-HasMorePages", "true");
                    Response.Headers.Add("X-NextPage", $"{Request.Path}?query={Uri.EscapeDataString(query)}&page={page + 1}");
                }
                else
                {
                    Response.Headers.Add("X-HasMorePages", "false");
                }

                _logger.LogInformation("Found {GymCount} gyms with query: {query}", gyms.Count(), query);
                return Ok(gyms);
            }
            catch (GymNotFoundError ex)
            {
                _logger.LogWarning("No gyms found with query: {query}", query);
                return NotFound(ex.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while searching gyms with query: {query}", query);
                return StatusCode(500, "An error occurred while searching gyms.");
            }
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> FindGymsNearby([FromQuery] double latitude, [FromQuery] double longitude)
        {
            try
            {
                var gyms = await _gymService.FindManyNearbyAsync(latitude, longitude);
                _logger.LogInformation("Found {GymCount} gyms nearby.", gyms.Count());
                return Ok(gyms);
            }
            catch (GymNotFoundError ex)
            {
                _logger.LogWarning("No gyms found nearby.");
                return NotFound(ex.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while searching gyms nearby.");
                return StatusCode(500, "An error occurred while searching gyms nearby.");
            }
        }
    }
}