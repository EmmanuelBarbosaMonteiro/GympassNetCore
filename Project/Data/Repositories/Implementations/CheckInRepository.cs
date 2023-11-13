
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;

namespace ApiGympass.Data.Repositories.Implementations
{
    public class CheckInRepository : ICheckInRepository
    {
        private readonly GympassContext _context;

        public CheckInRepository(GympassContext context)
        {
            _context = context;
        }

        public async Task<CheckIn> CreateCheckInAsync(CheckIn checkIn)
        {
            await _context.CheckIns.AddAsync(checkIn);
            await _context.SaveChangesAsync();

            return checkIn;
        }

        public async Task<CheckIn> GetCheckInByIdAsync(Guid checkInId)
        {
            return await _context.CheckIns.FindAsync(checkInId);
        }
    }
}