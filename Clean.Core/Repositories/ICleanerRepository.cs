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
        public Task<List<Cleaners>> GetAllCleanersAsync();
        public Task<Cleaners> GetCleanerByIdAsync(int id);
        public Task AddCleanerAsync(Cleaners newCleaner);
        public Task UpdateCleanerAsync(Cleaners updatedCleaner);
        public Task DeleteCleanerAsync(int id);
        public Task<Cleaners> GetCleanerByIDOfCleanerAsync(int IdOfCleaner);
        public Task<Cleaners?> GetCleanerByUserIdAsync(int userId);

        Task SaveChangesAsync();


    }
}
