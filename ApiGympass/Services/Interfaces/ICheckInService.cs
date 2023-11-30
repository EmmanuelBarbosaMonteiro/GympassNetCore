using ApiGympass.Data.Dtos;
using ApiGympass.Models;

namespace ApiGympass.Services.Interfaces
{
    public interface ICheckInService
    {
        Task<ReadCheckInDto> CreateCheckInAsync(CreateCheckInDto createCheckInDto);
        Task<ReadCheckInDto> ValidatedCheckIn(Guid checkInId);
        Task<ReadCheckInDto?> GetCheckInByIdAsync(Guid checkInId);
        Task<(IEnumerable<ReadCheckInDto>, bool)> GetCheckInsByUserIdAsync(Guid userId, int page);
        Task<int?> GetCheckInsUserMetricsByUserId(Guid userId);
    }
}