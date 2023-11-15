using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using ApiGympass.Services.ErrorHandling;
using ApiGympass.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiGympass.Services.Implementations
{
    public class ArchivingUserService : IUserService
    {
        private readonly IUserService _decoratedUserService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ArchivingUserService> _logger;

        public ArchivingUserService(IUserService decoratedUserService, UserManager<User> userManager, ILogger<ArchivingUserService> logger)
        {
            _decoratedUserService = decoratedUserService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ReadUserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            return await _decoratedUserService.CreateUserAsync(createUserDto);
        }

        public async Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.State == State.Inactive)
            {
                _logger.LogWarning("Attempted to get a non-existent user.");
                throw new UserNotFoundError();
            }

            _logger.LogInformation("User updated with ID: {UserId}", user.Id);
            return await _decoratedUserService.UpdateUserAsync(userId, updateUserDto);
        }

        public async Task<string> LoginUserAsync(LoginUserDto loginUserDto)
        {
            var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
            if (user == null || user.State == State.Inactive)
            {
                _logger.LogWarning("Attempted to get a non-existent user.");
                throw new UserNotFoundError();
            }

            _logger.LogInformation("User logged in with ID: {UserId}", user.Id);
            return await _decoratedUserService.LoginUserAsync(loginUserDto);
        }

        public async Task<IdentityResult> PatchUserAsync(string userId, JsonPatchDocument<UpdateUserDto> patchDocument)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.State == State.Inactive)
            {
                _logger.LogWarning("Attempted to get a non-existent user.");
                throw new UserNotFoundError();
            }

            _logger.LogInformation("User updated with ID: {UserId}", user.Id);
            return await _decoratedUserService.PatchUserAsync(userId, patchDocument);
        }

        public async Task<IEnumerable<ReadUserDto>> GetAllUsersAsync()
        {
            var users = await _decoratedUserService.GetAllUsersAsync();
            var activeUsers = users.Where(u => u.State == State.Active).ToList();

            _logger.LogInformation("Retrieved {UserCount} users.", activeUsers.Count);
            return activeUsers;
        }

        public async Task<ReadUserDto> GetByIdAsync(Guid userId)
        {
            var user = await _decoratedUserService.GetByIdAsync(userId);
            if (user.State == State.Inactive)
            {
                _logger.LogWarning("Attempted to get a non-existent user.");
                throw new UserNotFoundError();
            }

            _logger.LogInformation("User retrieved with ID: {UserId}", user.Id);
            return user;
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.State == State.Inactive)
            {
                _logger.LogWarning("Attempted to get a non-existent user.");
                throw new UserNotFoundError();
            }
            
            user.State = State.Inactive;
            var result = await _userManager.UpdateAsync(user);

            _logger.LogInformation("User deleted with ID: {UserId}", user.Id);
            return result;
        }
    }
}