using ApiGympass.Models;

namespace ApiGympass.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> FindById(Guid userId);
        Task<IEnumerable<User>> GetAllAsync();
    }
}