using ApiGympass.Data.Dtos;
using ApiGympass.Models;

namespace ApiGympass.Services.Interfaces
{
    public interface IGymService
    {
        Task<ReadGymDto> CreateGymAsync(CreateGymDto gymDto);
        Task<ReadGymDto?> FindById(Guid gymId);
        Task<(IEnumerable<ReadGymDto>, bool)> SearchGymsAsync(string query, int page);
        Task<IEnumerable<Gym>> FindManyNearbyAsync(double latitude, double longitude);
    }
}