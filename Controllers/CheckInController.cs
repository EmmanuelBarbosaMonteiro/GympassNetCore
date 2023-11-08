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

        public CheckInController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckIn(CreateCheckInDto checkInDto)
        {
            try
            {
                var checkIn = await _checkInService.CreateCheckInAsync(checkInDto);

                return Ok("Check-in created successfully.");
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}