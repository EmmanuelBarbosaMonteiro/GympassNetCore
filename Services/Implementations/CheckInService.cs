using ApiGympass.Data.Dtos;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using ApiGympass.Services.Interfaces;
using AutoMapper;

namespace ApiGympass.Services.Implementations
{
    public class CheckInService : ICheckInService
    {
        private readonly ICheckInRepository _checkInRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CheckInService> _logger;

        public CheckInService(ICheckInRepository checkInRepository, IMapper mapper, IUserRepository userRepository, ILogger<CheckInService> logger)
        {
            _checkInRepository = checkInRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<CreateCheckInDto> CreateCheckInAsync(CreateCheckInDto checkInDto)
        {
            var user = await _userRepository.GetByIdAsync(checkInDto.UserId);
            
            if (user == null || user.IsDeleted)
            {
                throw new ArgumentException("User not found.");
            }

            try
            {
                var checkIn = _mapper.Map<CheckIn>(checkInDto);

                await _checkInRepository.CreateCheckInAsync(checkIn);

                var createCheckInDto = _mapper.Map<CreateCheckInDto>(checkIn);

                _logger.LogInformation("CheckIn created with ID: {CheckInId}", checkIn.Id);

                return createCheckInDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating check-in.");
                throw;
            }
        }

        public async Task<ReadCheckInDto> GetCheckInByIdAsync(Guid checkInId)
        {
            _logger.LogInformation("Retrieving check-in with ID: {CheckInId}", checkInId);

            try
            {
                var checkIn = await _checkInRepository.GetCheckInByIdAsync(checkInId);

                if (checkIn == null)
                {
                    _logger.LogWarning("No check-in found with ID: {CheckInId}", checkInId);
                    return null;
                }

                var readCheckInDto = _mapper.Map<ReadCheckInDto>(checkIn);

                return readCheckInDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while retrieving check-in.");
                throw;
            }
        }
    }
}