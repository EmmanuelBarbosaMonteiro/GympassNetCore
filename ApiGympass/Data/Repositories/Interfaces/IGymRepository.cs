using ApiGympass.Models;

namespace ApiGympass.Data.Repositories.Interfaces
{
    public interface IGymRepository
    {
        Task<Gym> CreateGymAsync(Gym gym);
        Task<Gym?> FindById(Guid gymId);
    }
}