using Clean.Core.Entities;

namespace Clean.Core.Repositories
{
    public interface IBlockedDateRepository
    {
        Task<List<BlockedDate>> GetFromDateAsync(int cleanerId, DateOnly from);
        Task<List<DateOnly>> GetDatesInRangeAsync(int cleanerId, DateOnly from, DateOnly to);
        Task AddRangeAsync(IEnumerable<BlockedDate> dates);
        Task RemoveRangeAsync(int cleanerId, DateOnly from, DateOnly to);
        Task SaveAsync();
    }
}
