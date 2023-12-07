using ApiGympass.Models;

namespace ApiGympass.Data.Repositories.Interfaces
{
    public interface IGymRepository
    {
        Task<Gym> CreateGymAsync(Gym gym);
        Task<Gym?> FindById(Guid gymId);
        Task<IEnumerable<Gym>> SearchManyAsync(string query, int page, int pageSize);
        Task<IEnumerable<Gym>> FindManyNearbyAsync(decimal latitude, decimal longitude);
        Task<int> CountAsync(string query);

    }
}