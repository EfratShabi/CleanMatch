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
    public class CleanerRepository: ICleanerService
    {
        private readonly ICleanerRepository _cleanerRepository;
        public CleanerRepository(ICleanerRepository cleanerRepository)
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
    }
}
