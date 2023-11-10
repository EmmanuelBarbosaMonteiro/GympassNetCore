using ApiGympass.Data.Dtos;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using AutoMapper;
using ApiGympass.Services.Interfaces;

namespace ApiGympass.Services.Implementations
{
    public class GymService : IGymService
    {
        private readonly IGymRepository _gymRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GymService> _logger;


        public GymService(IGymRepository gymRepository, IMapper mapper, ILogger<GymService> logger)
        {
            _gymRepository = gymRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<Gym> CreateGymAsync(CreateGymDto gymDto)
        {
           if (gymDto == null)
            {
                _logger.LogError("CreateGymAsync called with null DTO.");
                throw new ArgumentNullException(nameof(gymDto));
            }

            try
            {
                var gym = _mapper.Map<Gym>(gymDto);

                await _gymRepository.CreateGymAsync(gym);

                _logger.LogInformation("Gym created with ID: {GymId}", gym.Id);
                
                return gym;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating gym.");
                throw;
            }
        }

        public async Task<Gym> GetGymByIdAsync(Guid gymId)
        {
            _logger.LogInformation("Retrieving gym with ID: {GymId}", gymId);

            try
            {
                var gym = await _gymRepository.GetGymByIdAsync(gymId);

                if (gym == null)
                {
                    _logger.LogWarning("No gym found with ID: {GymId}", gymId);
                    return null;
                }

                return gym;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while retrieving gym with ID: {GymId}", gymId);
                throw;
            }
        }

    }
}