
using ApiGympass.Models;

namespace ApiGympass.Data.Repositories.Interfaces
{
    public interface ICheckInRepository
    {
        Task<CheckIn> CreateCheckInAsync(CheckIn checkIn);
        Task<CheckIn?> FindById(Guid checkInId);
        Task<CheckIn?> FindByUserIdOnDate(Guid userId, DateTime date);
    }
}