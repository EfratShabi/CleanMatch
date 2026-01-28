using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.Services
{
    public interface IScheduleService
    {
        Task<List<Schedule>> GetAllAsync();
        Task<Schedule?> GetByIdAsync(int id);
        Task AddScheduleAsync(Schedule newSchedule);
        Task UpdateScheduleAsync(Schedule updatedSchedule);
        Task DeleteScheduleAsync(int id);
    }
}