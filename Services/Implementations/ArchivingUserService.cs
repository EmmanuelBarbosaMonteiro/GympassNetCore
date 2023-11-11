using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using ApiGympass.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiGympass.Services.Implementations
{
    public class ArchivingUserService : IUserService
    {
        private readonly IUserService _decoratedUserService;
        private readonly UserManager<User> _userManager;

        public ArchivingUserService(IUserService decoratedUserService, UserManager<User> userManager)
        {
            _decoratedUserService = decoratedUserService;
            _userManager = userManager;
        }

        public async Task<(IdentityResult Result, ReadUserDto ReadUserDto)> CreateUserAsync(CreateUserDto createUserDto)
        {
            return await _decoratedUserService.CreateUserAsync(createUserDto);
        }

        public async Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.State == State.Inactive)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found or deleted." });
            }

            return await _decoratedUserService.UpdateUserAsync(userId, updateUserDto);
        }

        public async Task<IdentityResult> PatchUserAsync(string userId, JsonPatchDocument<UpdateUserDto> patchDocument)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.State == State.Inactive)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found or is inactive." });
            }

            return await _decoratedUserService.PatchUserAsync(userId, patchDocument);
        }

        public async Task<IEnumerable<ReadUserDto>> GetAllUsersAsync()
        {
            var users = await _decoratedUserService.GetAllUsersAsync();
            var activeUsers = users.Where(u => u.State == State.Active).ToList();

            return activeUsers;
        }

        public async Task<ReadUserDto> GetByIdAsync(Guid userId)
        {
            var user = await _decoratedUserService.GetByIdAsync(userId);
            if (user.State == State.Inactive)
            {
                return null;
            }

            return user;
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.State == State.Inactive)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found or deleted." });
            }
            
            user.State = State.Inactive;
            var result = await _userManager.UpdateAsync(user);

            return result;
        }
    }
}