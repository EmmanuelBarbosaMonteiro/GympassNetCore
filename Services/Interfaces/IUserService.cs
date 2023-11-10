using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiGympass.Services.Interfaces
{
    public interface IUserService
    {
        Task<(IdentityResult Result, ReadUserDto ReadUserDto)> CreateUserAsync(CreateUserDto createUserDto);
        Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
        Task<IdentityResult> PatchUserAsync(string userId, JsonPatchDocument<UpdateUserDto> patchDocument);
        Task<IdentityResult> GetByIdAsync(Guid userId);
        Task<IEnumerable<ReadUserDto>> GetAllUsersAsync();
        Task<IdentityResult> DeleteUserAsync(string userId);
    }
}