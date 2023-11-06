using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using ApiGympass.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApiGympass.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
         private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> UserRegister(CreateUserDto dto)
        {
            var result = await _userService.CreateUserAsync(dto);

            if (result.Succeeded)
            {
                return Ok("User created successfully");
            }

            return BadRequest("An error occurred");
        }
    }
}