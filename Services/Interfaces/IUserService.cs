using ApiGympass.Data.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiGympass.Services.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> CreateUserAsync(CreateUserDto createUserDto);
        Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
        Task<IdentityResult> PatchUserAsync(string userId, JsonPatchDocument<UpdateUserDto> patchDocument);
        Task<ReadUserDto> GetByIdAsync(Guid userId);
        Task<IEnumerable<ReadUserDto>> GetAllUsersAsync();
    }
}