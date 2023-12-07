
using ApiGympass.Models;

namespace ApiGympass.Data.Repositories.Interfaces
{
    public interface ICheckInRepository
    {
        Task<CheckIn> CreateCheckInAsync(CheckIn checkIn);
        Task<CheckIn>UpdateCheckIn(CheckIn checkIn);
        Task<CheckIn?> FindById(Guid checkInId);
        Task<CheckIn?> FindByUserIdOnDate(Guid userId, DateTime date);
        Task<IEnumerable<CheckIn>> FindManyByUserId(Guid userId, int page, int pageSize);
        public Task<int> CountCheckInsByUserId(Guid userId);
    }
}