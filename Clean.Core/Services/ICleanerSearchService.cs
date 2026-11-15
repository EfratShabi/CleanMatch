using Clean.Core.Entities;
using Clean.Core.Models;

namespace Clean.Core.Services
{
    public interface ICleanerSearchService
    {
        Task<List<CleanerMatch>> SearchAsync(Customers customer, DateOnly date, TimeOnly start, double hours, int? maxPricePerHour);
    }
}
