using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;

namespace ApiGympass.Data.Repositories.Implementations
{
    public class GymRepository : IGymRepository
    {
        private readonly GympassContext _context;

        public GymRepository(GympassContext context)
        {
            _context = context;
        }
        
        public async Task<Gym> CreateGymAsync(Gym gym)
        {
            await _context.Gyms.AddAsync(gym);
            await _context.SaveChangesAsync();

            return gym;
        }

        public async Task<Gym?> FindById(Guid gymId)
        {
            return await _context.Gyms.FindAsync(gymId);
        }
    }
}