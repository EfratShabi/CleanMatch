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
    }
}
