using ApiGympass.Data.Dtos;
using ApiGympass.Models;

namespace ApiGympass.Services.Interfaces
{
    public interface IGymService
    {
        Task<Gym> CreateGymAsync(CreateGymDto gymDto);
    }
}