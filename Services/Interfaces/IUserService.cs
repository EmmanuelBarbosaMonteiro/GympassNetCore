using ApiGympass.Data.Dtos;
using Microsoft.AspNetCore.Identity;

namespace ApiGympass.Services.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> CreateUserAsync(CreateUserDto createUserDto);
    }
}