using ApiGympass.Data.Dtos;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using ApiGympass.Services.Interfaces;

namespace ApiGympass.Services.Implementations
{
    public class GymService : IGymService
    {
        private readonly IGymRepository _gymRepository;

        public GymService(IGymRepository gymRepository)
        {
            _gymRepository = gymRepository;
        }
        
        public async Task<Gym> CreateGymAsync(CreateGymDto gymDto)
        {
           var gym = new Gym
           {
                Title = gymDto.Title,
                Latitude = gymDto.Latitude,
                Longitude = gymDto.Longitude
           };

            await _gymRepository.CreateGymAsync(gym);
            
            return gym;
        }
    }
}