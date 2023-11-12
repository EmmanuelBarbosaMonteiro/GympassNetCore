using ApiGympass.Data.Dtos;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using ApiGympass.Services.ErrorHandling;
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
                    _logger.LogWarning("User already exists.");
                    throw new UserAlreadyExistsError();
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
                throw new UserNotFoundError();
            }

            _mapper.Map(updateUserDto, user);

            var result = await _userManager.UpdateAsync(user);

            _logger.LogInformation("User updated with ID: {UserId}", user.Id);
            return result;
        }

        public async Task<IdentityResult> PatchUserAsync(string userId, JsonPatchDocument<UpdateUserDto> patchDocument)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("Attempted to update a non-existent user.");
                throw new UserNotFoundError();
            }

            var userDto = _mapper.Map<UpdateUserDto>(user);

            patchDocument.ApplyTo(userDto);

            _mapper.Map(userDto, user);

            var result = await _userManager.UpdateAsync(user);

            _logger.LogInformation("User updated with ID: {UserId}", user.Id);
            return result;
        }

        public async Task<ReadUserDto> GetByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                _logger.LogWarning("Attempted to get a non-existent user.");
                throw new UserNotFoundError();
            }

            _logger.LogInformation("User retrieved with ID: {UserId}", user.Id);
            return user == null ? null : _mapper.Map<ReadUserDto>(user);
        }

        public async Task<IEnumerable<ReadUserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            _logger.LogInformation("Users retrieved.");
            return users.Select(user => _mapper.Map<ReadUserDto>(user)).ToList();
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                _logger.LogWarning("Attempted to delete a non-existent user.");
                throw new UserNotFoundError();;
            }
            
            var result = await _userManager.UpdateAsync(user);
            
            _logger.LogInformation("User deleted with ID: {UserId}", user.Id);
            return result;
        }
    }
}