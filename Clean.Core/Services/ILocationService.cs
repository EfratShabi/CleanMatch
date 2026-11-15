namespace Clean.Core.Repositories
{
    public interface ILocationService
    {
        Task<(double lat, double lon)?> GetCoordinatesAsync(string city, string street, string zipCode);
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    }
}
