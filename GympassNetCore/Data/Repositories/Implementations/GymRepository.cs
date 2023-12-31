using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<Gym>> SearchManyAsync(string query, int page, int pageSize)
        {
            return await _context.Gyms
                    .Where(gym => (gym.Title ?? string.Empty).ToLower().Contains(query.ToLower()))
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
        }

        public async Task<int> CountAsync(string query)
        {
            return await _context.Gyms
                        .CountAsync(gym => (gym.Title ?? string.Empty).ToLower().Contains(query.ToLower()));
        }

        public async Task<IEnumerable<Gym>> FindManyNearbyAsync(decimal latitude, decimal longitude)
        {
            var gyms = await _context.Gyms
                .FromSqlRaw("SELECT * FROM \"Gyms\" WHERE (6371 * acos(cos(radians({0})) * cos(radians(\"Latitude\")) * cos(radians(\"Longitude\") - radians({1})) + sin(radians({0})) * sin(radians(\"Latitude\")))) <= 10", latitude, longitude)
                .ToListAsync();
                
            return gyms;
        }
    }
}