using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using ApiGympass.Utils;

namespace ApiGympass.Services.Interfaces
{
    public interface IGymService
    {
        Task<ServiceResult<Gym>> CreateGymAsync(CreateGymDto gymDto);
    }
}