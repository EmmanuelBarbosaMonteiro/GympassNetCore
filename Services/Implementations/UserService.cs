using ApiGympass.Data.Dtos;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using ApiGympass.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiGympass.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<CheckInService> _logger;

        public UserService(IUserRepository userRepository, UserManager<User> userManager, IMapper mapper, ILogger<CheckInService> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(IdentityResult Result, ReadUserDto ReadUserDto)> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);

            try
            {
                var result = await _userManager.CreateAsync(user, createUserDto.Password);

                if (result.Succeeded)
                {
                    var readUserDto = _mapper.Map<ReadUserDto>(user);

                    _logger.LogInformation("User created with ID: {UserId}", user.Id);

                    return (result, readUserDto);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning($"CreateUser: Failed to create user - {errors}");
                    return (result, null);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating user.");
                throw;
            }
        }

        public async Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("Attempted to update a non-existent user.");
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            _mapper.Map(updateUserDto, user);

            var result = await _userManager.UpdateAsync(user);

            return result;
        }

        public async Task<IdentityResult> PatchUserAsync(string userId, JsonPatchDocument<UpdateUserDto> patchDocument)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("Attempted to update a non-existent user.");
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            var userDto = _mapper.Map<UpdateUserDto>(user);

            patchDocument.ApplyTo(userDto);

            _mapper.Map(userDto, user);

            var result = await _userManager.UpdateAsync(user);

            return result;
        }

        public async Task<ReadUserDto> GetByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                return null;
            }

            return user == null ? null : _mapper.Map<ReadUserDto>(user);
        }

        public async Task<IEnumerable<ReadUserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(user => _mapper.Map<ReadUserDto>(user)).ToList();
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }
            
            var result = await _userManager.UpdateAsync(user);
            
            return result;
        }
    }
}