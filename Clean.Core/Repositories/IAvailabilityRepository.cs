using Clean.Core.Entities;

namespace Clean.Core.Repositories
{
    public interface IAvailabilityRepository
    {
        Task<List<Availability>> GetByCleanerIdAsync(int cleanerId);
        Task ReplaceForCleanerAsync(int cleanerId, List<Availability> slots);
        Task SaveAsync();
    }
}
