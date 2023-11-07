using ApiGympass.Data.Dtos;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using ApiGympass.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace ApiGympass.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, UserManager<User> userManager, IMapper mapper)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IdentityResult> CreateUserAsync(CreateUserDto dto)
        {
            var user = _mapper.Map<User>(dto);
            
            var result = await _userManager.CreateAsync(user, dto.Password);


            return result;
        }

        public async Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            _mapper.Map(updateUserDto, user);

            var result = await _userManager.UpdateAsync(user);

            return result;
        }
    }
}