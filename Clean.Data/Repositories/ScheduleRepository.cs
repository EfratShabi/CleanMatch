using Clean.Core.Entities;
using Clean.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Data.Repositories
{
    public class ScheduleRepository:IScheduleRepository
    {
        private readonly DataContext _context;
        public ScheduleRepository(DataContext context)
        {
            _context = context;
        }
        public List<Schedule> GetAll()
        {
            return _context.Schedule.ToList();

        }
        public Schedule GetById(int id)
        {
            var schedule = _context.Schedule.ToList().Find(schedule => schedule.Id == id);
            return schedule;
        }
    }
}
