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
    public class CleanerService: ICleanerService
    {
        private readonly ICleanerRepository _cleanerRepository;
        public CleanerService(ICleanerRepository cleanerRepository)
        {
            _cleanerRepository = cleanerRepository;
        }
        public List<Cleaners> GetAllCleaners()
        {
            return _cleanerRepository.GetAllCleaners();
        }
        public Cleaners GetCleanerById(int id)
        {
            return _cleanerRepository.GetCleanerById(id);
        }
        public Cleaners GetCleanerByIDOfCleaner(int IdOfCleaner)
        {
            return _cleanerRepository.GetCleanerById(IdOfCleaner);
        }
        public void AddCleaner(Cleaners newCleaner)
        {
            _cleanerRepository.AddCleaner(newCleaner);
            _cleanerRepository.Save();
        }
        public void UpdateCleaner(Cleaners updatedCleaner)
        {
            _cleanerRepository.UpdateCleaner(updatedCleaner);
            _cleanerRepository.Save();
        }
        public void DeleteCleaner(int id)
        {
            _cleanerRepository.DeleteCleaner(id);
            _cleanerRepository.Save();
        }

    }
}
