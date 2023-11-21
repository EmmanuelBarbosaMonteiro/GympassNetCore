
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<CheckIn?> GetCheckInByIdAsync(Guid checkInId)
        {
            return await _context.CheckIns.FindAsync(checkInId);
        }

        public async Task<CheckIn?> FindByUserIdOnDate(Guid userId, DateTime date)
        {
            return await _context.CheckIns
                .FirstOrDefaultAsync(checkIn => checkIn.UserId == userId && checkIn.CreatedAt.Date == date.Date);
        }
    }
}