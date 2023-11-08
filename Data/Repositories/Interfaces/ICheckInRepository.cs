
using ApiGympass.Models;

namespace ApiGympass.Data.Repositories.Interfaces
{
    public interface ICheckInRepository
    {
        Task<CheckIn> CreateCheckInAsync(CheckIn checkIn);
    }
}