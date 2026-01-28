using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clean.Core.Repositories
{
    public interface IScheduleRepository
    {
        Task<List<Schedule>> GetAllAsync();
        Task<Schedule?> GetByIdAsync(int id);
        Task AddScheduleAsync(Schedule newSchedule);
        Task UpdateScheduleAsync(Schedule updatedSchedule);
        Task DeleteScheduleAsync(int id);
        Task SaveAsync();
    }
}