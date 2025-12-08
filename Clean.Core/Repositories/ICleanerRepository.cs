using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.Repositories
{
    public interface ICleanerRepository
    {
        public List<Cleaners> GetAllCleaners();
        public Cleaners GetCleanerById(int id);
        public void AddCleaner(Cleaners newCleaner);
        public void UpdateCleaner(Cleaners updatedCleaner);
        public void DeleteCleaner(int id);
        public Cleaners GetCleanerByIDOfCleaner(int IdOfCleaner);

        public void Save();



    }
}
