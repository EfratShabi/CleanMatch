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
    public class ScheduleService:IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }
        public List<Schedule> GetAll()
        {
            return _scheduleRepository.GetAll();
        }
        public Schedule GetById(int id)
        {
            return _scheduleRepository.GetById(id);
        }
        public void AddSchedule(Schedule newSchedule)
        {
            _scheduleRepository.AddSchedule(newSchedule);
            _scheduleRepository.Save();
        }
        public void UpdateSchedule(Schedule updatedSchedule)
        {
            _scheduleRepository.UpdateSchedule(updatedSchedule);
            _scheduleRepository.Save();
        }
        public void DeleteSchedule(int id)
        {
            _scheduleRepository.DeleteSchedule(id);
            _scheduleRepository.Save();
        }
    }
}
