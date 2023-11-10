using ApiGympass.Data.Dtos;
using ApiGympass.Models;

namespace ApiGympass.Services.Interfaces
{
    public interface ICheckInService
    {
        Task<CreateCheckInDto> CreateCheckInAsync(CreateCheckInDto checkInDto);
        Task<ReadCheckInDto> GetCheckInByIdAsync(Guid checkInId);
    }
}