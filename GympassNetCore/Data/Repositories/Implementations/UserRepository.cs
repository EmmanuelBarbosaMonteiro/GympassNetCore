using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiGympass.Data.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly GympassContext _context;

        public UserRepository(GympassContext context)
        {
            _context = context;
        }

        public async Task<User?> FindById(Guid userId)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}