
using ApiGympass.Models;

namespace ApiGympass.Data.Repositories.Interfaces
{
    public interface ICheckInRepository
    {
        Task<CheckIn> CreateCheckInAsync(CheckIn checkIn);
        Task<CheckIn> GetCheckInByIdAsync(Guid checkInId);
        Task<CheckIn?> FindByUserIdOnDate(Guid userId, DateTime date);
    }
}