using ApiGympass.Data.Dtos;
using ApiGympass.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiGympass.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GymController : ControllerBase
    {
        private readonly IGymService _gymService;

        public GymController(IGymService gymService, ILogger<GymController> logger)
        {
            _gymService = gymService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGym(CreateGymDto gymDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = await _gymService.CreateGymAsync(gymDto);

            if (result.IsSuccess)
            {
                return Ok(result.Entity.Id);
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
    }
}