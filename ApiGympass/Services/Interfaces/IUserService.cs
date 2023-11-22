using ApiGympass.Data.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiGympass.Services.Interfaces
{
    public interface IUserService
    {
        Task<ReadUserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<string> LoginUserAsync(LoginUserDto loginUserDto);
        Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
        Task<IdentityResult> PatchUserAsync(string userId, JsonPatchDocument<UpdateUserDto> patchDocument);
        Task<ReadUserDto?> GetByIdAsync(Guid userId);
        Task<IEnumerable<ReadUserDto>> GetAllUsersAsync();
        Task<IdentityResult> DeleteUserAsync(string userId);
    }
}