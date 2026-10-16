using Clean.Core.Entities;
using Clean.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Data
{
    public class CleanerRepository : ICleanerRepository
    {
        private readonly DataContext _context;

        public CleanerRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Cleaners>> GetAllCleanersAsync()
        {
            return await _context.Cleaners
                .Include(c => c.ArrFidback)
                .Include(c => c.ArrAvailability)
                .ToListAsync();
        }

        public async Task<Cleaners> GetCleanerByIdAsync(int id)
        {
            return await _context.Cleaners
                .Include(c => c.ArrFidback)
                .Include(c => c.ArrAvailability)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddCleanerAsync(Cleaners newCleaner)
        {
            await _context.Cleaners.AddAsync(newCleaner);
        }

        public async Task<Cleaners> GetCleanerByIDOfCleanerAsync(int IdOfCleaner)
        {
            return await _context.Cleaners.FirstOrDefaultAsync(c => c.IdOfCleaner == IdOfCleaner);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCleanerAsync(Cleaners updatedCleaner)
        {
            _context.Cleaners.Update(updatedCleaner);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCleanerAsync(int id)
        {
            var cleaner = await GetCleanerByIdAsync(id);
            if (cleaner != null)
            {
                _context.Cleaners.Remove(cleaner);
            }
        }
    }
}
