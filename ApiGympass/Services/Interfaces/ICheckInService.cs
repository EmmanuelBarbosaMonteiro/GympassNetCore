using ApiGympass.Data.Dtos;
using ApiGympass.Models;

namespace ApiGympass.Services.Interfaces
{
    public interface ICheckInService
    {
        Task<ReadCheckInDto> CreateCheckInAsync(CreateCheckInDto createCheckInDto);
        Task<ReadCheckInDto?> GetCheckInByIdAsync(Guid checkInId);
    }
}