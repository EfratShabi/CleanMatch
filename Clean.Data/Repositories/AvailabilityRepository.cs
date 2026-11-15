using Clean.Core.Entities;
using Clean.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clean.Data.Repositories
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly DataContext _context;

        public AvailabilityRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Availability>> GetByCleanerIdAsync(int cleanerId)
        {
            return await _context.Availabilities
                .Where(a => a.CleanerId == cleanerId)
                .OrderBy(a => a.DayOfWeek).ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task ReplaceForCleanerAsync(int cleanerId, List<Availability> slots)
        {
            var existing = await _context.Availabilities.Where(a => a.CleanerId == cleanerId).ToListAsync();
            _context.Availabilities.RemoveRange(existing);
            await _context.Availabilities.AddRangeAsync(slots);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
