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
            return _context.Cleaners;
        }
        public Cleaners GetCleanerById(int id)
        {
            var s = _context.Cleaners.Find(c => c.IdOfCleaner == id);
            return s;
        }

    }
}
