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
        public void AddSchedule(Schedule newSchedule)
        {
            _context.Schedule.Add(newSchedule);
        }
        public void UpdateSchedule(Schedule updatedSchedule)
        {
            _context.Schedule.Update(updatedSchedule);
        }
        public void DeleteSchedule(int id)
        {
            _context.Schedule.Remove(GetById(id));
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
