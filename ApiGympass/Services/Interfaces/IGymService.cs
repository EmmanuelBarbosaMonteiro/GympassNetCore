using ApiGympass.Data.Dtos;
using ApiGympass.Models;

namespace ApiGympass.Services.Interfaces
{
    public interface IGymService
    {
        Task<ReadGymDto> CreateGymAsync(CreateGymDto gymDto);
        Task<ReadGymDto?> FindById(Guid gymId);
    }
}