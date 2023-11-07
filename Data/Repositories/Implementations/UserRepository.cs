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

        public async Task<User> GetByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}