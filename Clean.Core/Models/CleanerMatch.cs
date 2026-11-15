using Clean.Core.Entities;

namespace Clean.Core.Models
{
    public record CleanerMatch(Cleaners Cleaner, double DistanceKm, double Score, double TotalPrice);
}
