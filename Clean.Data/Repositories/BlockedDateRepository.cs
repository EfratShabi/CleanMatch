using Clean.Core.Entities;
using Clean.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clean.Data.Repositories
{
    public class BlockedDateRepository : IBlockedDateRepository
    {
        private readonly DataContext _context;

        public BlockedDateRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<BlockedDate>> GetFromDateAsync(int cleanerId, DateOnly from)
        {
            return await _context.BlockedDates
                .Where(b => b.CleanerId == cleanerId && b.Date >= from)
                .OrderBy(b => b.Date)
                .ToListAsync();
        }

        public async Task<List<DateOnly>> GetDatesInRangeAsync(int cleanerId, DateOnly from, DateOnly to)
        {
            return await _context.BlockedDates
                .Where(b => b.CleanerId == cleanerId && b.Date >= from && b.Date <= to)
                .Select(b => b.Date)
                .ToListAsync();
        }

        public async Task<HashSet<int>> GetCleanerIdsBlockedOnAsync(DateOnly date)
        {
            var ids = await _context.BlockedDates
                .Where(b => b.Date == date)
                .Select(b => b.CleanerId)
                .ToListAsync();
            return ids.ToHashSet();
        }

        public async Task AddRangeAsync(IEnumerable<BlockedDate> dates)
        {
            await _context.BlockedDates.AddRangeAsync(dates);
        }

        public async Task RemoveRangeAsync(int cleanerId, DateOnly from, DateOnly to)
        {
            var existing = await _context.BlockedDates
                .Where(b => b.CleanerId == cleanerId && b.Date >= from && b.Date <= to)
                .ToListAsync();
            _context.BlockedDates.RemoveRange(existing);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
