using Clean.Core.Entities;

namespace Clean.Core.Services
{
    public interface IBlockedDateService
    {
        Task<List<BlockedDate>> GetUpcomingAsync(int cleanerId);
        Task BlockAsync(int cleanerId, DateOnly from, DateOnly to);
        Task UnblockAsync(int cleanerId, DateOnly from, DateOnly to);
    }
}
