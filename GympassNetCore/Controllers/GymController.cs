using ApiGympass.Data.Dtos;
using ApiGympass.Services.ErrorHandling;
using ApiGympass.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Creates a new gym.
        /// </summary>
        /// <remarks>
        /// This method creates a new gym with the provided details. Access is restricted to administrators.
        /// </remarks>
        /// <param name="gymDto">The gym data transfer object containing the gym's details.</param>
        /// <response code="201">If the gym is created successfully.</response>
        /// <response code="400">If the request has invalid model state.</response>
        /// <response code="403">If the user does not have administrator privileges.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ProducesResponseType(typeof(ReadGymDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Retrieves a gym by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the gym.</param>
        /// <response code="200">If the gym is found.</response>
        /// <response code="404">If no gym is found with the specified ID.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReadGymDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Searches for gyms based on a query.
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <param name="page">The page number for pagination (default is 1).</param>
        /// <response code="200">If gyms are found matching the query.</response>
        /// <response code="404">If no gyms are found matching the query.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<ReadGymDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Finds gyms nearby based on latitude and longitude.
        /// </summary>
        /// <param name="latitude">The latitude coordinate.</param>
        /// <param name="longitude">The longitude coordinate.</param>
        /// <response code="200">If gyms are found nearby.</response>
        /// <response code="404">If no gyms are found nearby.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [HttpGet("nearby")]
        [ProducesResponseType(typeof(IEnumerable<ReadGymDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindGymsNearby([FromQuery] decimal latitude, [FromQuery] decimal longitude)
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