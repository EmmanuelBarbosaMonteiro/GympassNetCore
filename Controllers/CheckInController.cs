using ApiGympass.Data;
using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiGympass.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckInController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly GympassContext _context;

        public CheckInController(IMapper mapper, GympassContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateCheckIn(CreateCheckInDto dto)
        {
            var checkIn = _mapper.Map<CheckIn>(dto);
            _context.CheckIns.Add(checkIn);
            _context.SaveChanges();

            return Ok();
        }
    }
}