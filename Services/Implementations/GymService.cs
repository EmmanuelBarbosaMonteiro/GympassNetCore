using ApiGympass.Data.Dtos;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using ApiGympass.Services.Interfaces;
using ApiGympass.Utils;

namespace ApiGympass.Services.Implementations
{
    public class GymService : IGymService
    {
        private readonly IGymRepository _gymRepository;

        public GymService(IGymRepository gymRepository)
        {
            _gymRepository = gymRepository;
        }
        
        public async Task<ServiceResult<Gym>> CreateGymAsync(CreateGymDto gymDto)
        {
           try
           {
                var gym = new Gym
                {
                    Title = gymDto.Title,
                    Latitude = gymDto.Latitude,
                    Longitude = gymDto.Longitude,
                    Description = gymDto.Description,
                    Phone = gymDto.Phone
                };

                await _gymRepository.CreateGymAsync(gym);

                return new ServiceResult<Gym>(true, gym, null);
           }
           catch (Exception e)
           {
               return new ServiceResult<Gym>(false, null, e.Message);
           }
        }
    }
}