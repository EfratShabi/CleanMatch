using Clean.Core.Entities;
using Clean.Core.Repositories;
using Clean.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Service
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<List<Schedule>> GetAllAsync()
        {
            return await _scheduleRepository.GetAllAsync();
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return await _scheduleRepository.GetByIdAsync(id);
        }

        public async Task AddScheduleAsync(Schedule newSchedule)
        {
            await _scheduleRepository.AddScheduleAsync(newSchedule);
            await _scheduleRepository.SaveAsync();
        }

        public async Task UpdateScheduleAsync(Schedule updatedSchedule)
        {
            await _scheduleRepository.UpdateScheduleAsync(updatedSchedule);
            await _scheduleRepository.SaveAsync();
        }

        public async Task DeleteScheduleAsync(int id)
        {
            await _scheduleRepository.DeleteScheduleAsync(id);
            await _scheduleRepository.SaveAsync();
        }
    }
}