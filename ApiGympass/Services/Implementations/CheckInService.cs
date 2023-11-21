using ApiGympass.Data.Dtos;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using ApiGympass.Services.ErrorHandling;
using ApiGympass.Services.Interfaces;
using AutoMapper;

namespace ApiGympass.Services.Implementations
{
    public class CheckInService : ICheckInService
    {
        private readonly ICheckInRepository _checkInRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ILogger<CheckInService> _logger;

        public CheckInService(ICheckInRepository checkInRepository, IMapper mapper, IUserService userService, ILogger<CheckInService> logger)
        {
            _checkInRepository = checkInRepository;
            _mapper = mapper;
            _userService = userService;
            _logger = logger;
        }

        public async Task<ReadCheckInDto> CreateCheckInAsync(CreateCheckInDto createCheckInDto)
        {
            try
            {
                var user = await _userService.GetByIdAsync(createCheckInDto.UserId);
                var checkIn = _mapper.Map<CheckIn>(createCheckInDto);

                var existingCheckIn = await _checkInRepository.FindByUserIdOnDate(createCheckInDto.UserId, createCheckInDto.CreatedAt);
                if (existingCheckIn != null)
                {
                    _logger.LogWarning("Check-in already exists for user with ID: {UserId} on date: {Date}", createCheckInDto.UserId, checkIn.CreatedAt);
                    throw new CheckInLimitExceeded();
                }

                await _checkInRepository.CreateCheckInAsync(checkIn);
                var readCheckInDto = _mapper.Map<ReadCheckInDto>(checkIn);
                _logger.LogInformation("CheckIn created with ID: {CheckInId}", checkIn.Id);

                return readCheckInDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating check-in.");
                throw;
            }
        }

        public async Task<ReadCheckInDto> GetCheckInByIdAsync(Guid checkInId)
        {

            try
            {
                var checkIn = await _checkInRepository.GetCheckInByIdAsync(checkInId);
                if (checkIn == null)
                {
                    _logger.LogWarning("No check-in found with ID: {CheckInId}", checkInId);
                    throw new CheckInNotFoundError();
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