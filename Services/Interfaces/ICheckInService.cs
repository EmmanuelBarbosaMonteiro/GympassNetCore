using ApiGympass.Data.Dtos;
using ApiGympass.Models;

namespace ApiGympass.Services.Interfaces
{
    public interface ICheckInService
    {
        Task<CheckIn> CreateCheckInAsync(CreateCheckInDto checkInDto);
    }
}