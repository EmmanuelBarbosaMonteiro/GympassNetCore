using ApiGympass.Data.Dtos;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using ApiGympass.Services.ErrorHandling;
using ApiGympass.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Project.Services.ErrorHandling;

namespace ApiGympass.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly TokenService _tokenService;

        public UserService(IUserRepository userRepository, UserManager<User> userManager, IMapper mapper, ILogger<UserService> logger, SignInManager<User> signInManager, TokenService tokenService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<ReadUserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                var user = _mapper.Map<User>(createUserDto);
                var result = await _userManager.CreateAsync(user, createUserDto.Password);

                if (result.Succeeded)
                {
                    var readUserDto = _mapper.Map<ReadUserDto>(user);
                    _logger.LogInformation("User created with ID: {UserId}", user.Id);
                    _tokenService.GenerateToken(user);
                    return readUserDto;
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

        public async Task<string> LoginUserAsync(LoginUserDto loginUserDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
                var result = await _signInManager.PasswordSignInAsync(user.UserName, loginUserDto.Password, false, false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in with ID: {UserId}", user.Id);
                    var token = _tokenService.GenerateToken(user);
                    return token;
                }
                else
                {
                    _logger.LogWarning("Invalid credentials.");
                    throw new InvalidCredentialsError();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while logging in user.");
                throw;
            }
        }

        public async Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            
            var user = await _userManager.FindByIdAsync(userId);
            _mapper.Map(updateUserDto, user);
            var result = await _userManager.UpdateAsync(user);
            _logger.LogInformation("User updated with ID: {UserId}", user.Id);
            return result;
        }

        public async Task<IdentityResult> PatchUserAsync(string userId, JsonPatchDocument<UpdateUserDto> patchDocument)
        {
            var user = await _userManager.FindByIdAsync(userId);

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
            var readUserDto = _mapper.Map<ReadUserDto>(user);
            _logger.LogInformation("User retrieved with ID: {UserId}", readUserDto.Id);
            return readUserDto;
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
            var result = await _userManager.UpdateAsync(user);
            _logger.LogInformation("User deleted with ID: {UserId}", user.Id);
            return result;
        }
    }
}