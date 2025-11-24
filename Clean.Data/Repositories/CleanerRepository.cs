using Clean.Core.Entities;
using Clean.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Data
{
    public class CleanerRepository: ICleanerRepository
    {
        private readonly DataContext _context;
        public CleanerRepository(DataContext context)
        {
            _context = context;
        }
        public List<Cleaners> GetAllCleaners()
        {
            return _context.Cleaners.ToList();
        }
        public Cleaners GetCleanerById(int id)
        {
            var s = _context.Cleaners.ToList().Find(c => c.Id == id);
            return s;
        }
        public void AddCleaner(Cleaners newCleaner)
        {
            _context.Cleaners.Add(newCleaner);
        }
        public void UpdateCleaner(Cleaners updatedCleaner)
        {
            _context.Cleaners.Update(updatedCleaner);
        }
        public void DeleteCleaner(int id)
        {
            _context.Cleaners.Remove(GetCleanerById(id));
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
