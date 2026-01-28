using Clean.Core.Entities;
using Clean.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Data.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly DataContext _context;

        public ScheduleRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Schedule>> GetAllAsync()
        {
            return await _context.Schedule.ToListAsync();
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return await _context.Schedule.FindAsync(id);
        }

        public async Task AddScheduleAsync(Schedule newSchedule)
        {
            await _context.Schedule.AddAsync(newSchedule);
        }

        public async Task UpdateScheduleAsync(Schedule updatedSchedule)
        {
            _context.Schedule.Update(updatedSchedule);
            await Task.CompletedTask;
        }

        public async Task DeleteScheduleAsync(int id)
        {
            var schedule = await GetByIdAsync(id);
            if (schedule != null)
            {
                _context.Schedule.Remove(schedule);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}