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
        private readonly ILocationService _locationService;

        public CleanerService(ICleanerRepository cleanerRepository, ILocationService locationService)
        {
            _cleanerRepository = cleanerRepository;
            _locationService = locationService;

        }
        public  async Task<List<Cleaners>> GetAllCleanersAsync()
        {
            return await  _cleanerRepository.GetAllCleanersAsync();
        }
        public  async Task<Cleaners> GetCleanerByIdAsync(int id)
        {
            return  await _cleanerRepository.GetCleanerByIdAsync(id);
        }
        public  async Task<Cleaners> GetCleanerByIDOfCleaner(int IdOfCleaner)
        {
            return await _cleanerRepository.GetCleanerByIDOfCleanerAsync(IdOfCleaner);
        }
        public async Task<Cleaners?> GetCleanerByUserIdAsync(int userId)
        {
            return await _cleanerRepository.GetCleanerByUserIdAsync(userId);
        }
        public async Task AddCleanerAsync(Cleaners newCleaner)
        {
           await _cleanerRepository.AddCleanerAsync(newCleaner);
           await _cleanerRepository.SaveChangesAsync();
            

        }
        public  async Task UpdateCleanerAsync(Cleaners updatedCleaner)
        {
            await _cleanerRepository.UpdateCleanerAsync(updatedCleaner);
           await _cleanerRepository.SaveChangesAsync();
        }
        public async Task DeleteCleanerAsync(int id)
        {
            await _cleanerRepository.DeleteCleanerAsync(id);
           await _cleanerRepository.SaveChangesAsync();
        }

    }
}
