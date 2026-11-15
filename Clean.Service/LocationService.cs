using Clean.Core.Repositories;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;

namespace Clean.Service
{
    public class LocationService : ILocationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LocationService> _logger;

        public LocationService(HttpClient httpClient, ILogger<LocationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<(double lat, double lon)?> GetCoordinatesAsync(string city, string street, string zipCode)
        {
            if (!string.IsNullOrWhiteSpace(city) && !string.IsNullOrWhiteSpace(street))
            {
                var byStreet = await SearchAsync($"city={Uri.EscapeDataString(city)}&street={Uri.EscapeDataString(street)}");
                if (byStreet != null) return byStreet;
            }

            if (!string.IsNullOrWhiteSpace(zipCode))
            {
                return await SearchAsync($"postalcode={Uri.EscapeDataString(zipCode)}");
            }

            return null;
        }

        private async Task<(double lat, double lon)?> SearchAsync(string query)
        {
            try
            {
                var response = await _httpClient.GetAsync($"search?{query}&country=Israel&format=json&limit=1");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Geocoding returned {Status}", (int)response.StatusCode);
                    return null;
                }

                using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var root = json.RootElement;
                if (root.GetArrayLength() == 0) return null;

                var first = root[0];
                return (double.Parse(first.GetProperty("lat").GetString()!, CultureInfo.InvariantCulture),
                        double.Parse(first.GetProperty("lon").GetString()!, CultureInfo.InvariantCulture));
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or FormatException or InvalidOperationException)
            {
                _logger.LogWarning(ex, "Geocoding request failed");
                return null;
            }
        }

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371;
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double angle) => Math.PI * angle / 180.0;
    }
}
