using ApiGympass.Data;
using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiGympass.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GymController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly GympassContext _context;

        public GymController(IMapper mapper, GympassContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGym(CreateGymDto dto)
        {
            var gym = _mapper.Map<Gym>(dto);
            _context.Gyms.Add(gym);
            await _context.SaveChangesAsync();
            return Ok(gym);
        }
    }
}