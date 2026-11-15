using Clean.Core.Entities;

namespace Clean.Core.Services
{
    public interface IAvailabilityService
    {
        Task<List<Availability>> GetByCleanerAsync(int cleanerId);
        Task ReplaceWeekAsync(int cleanerId, List<Availability> slots);
    }
}
