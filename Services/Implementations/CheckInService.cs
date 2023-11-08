using ApiGympass.Data.Dtos;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using ApiGympass.Services.Interfaces;

namespace ApiGympass.Services.Implementations
{
    public class CheckInService : ICheckInService
    {
        private readonly ICheckInRepository _checkInRepository;
        private readonly IUserRepository _userRepository;

        public CheckInService(ICheckInRepository checkInRepository, IUserRepository userRepository)
        {
            _checkInRepository = checkInRepository;
            _userRepository = userRepository;
        }

        public async Task<CheckIn> CreateCheckInAsync(CreateCheckInDto checkInDto)
        {
            var user = await _userRepository.GetByIdAsync(checkInDto.UserId);
            
            if (user == null || user.IsDeleted)
            {
                throw new InvalidOperationException("User not found.");
            }

            var checkIn = new CheckIn
            {
                UserId = checkInDto.UserId,
                GymId = checkInDto.GymId
            };

            await _checkInRepository.CreateCheckInAsync(checkIn);

            return checkIn;
        }
    }
}