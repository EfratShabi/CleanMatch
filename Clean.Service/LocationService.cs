using Clean.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Clean.Service
{
    public class LocationService : ILocationService
    {
        private readonly HttpClient _httpClient;
        public LocationService() 
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "CleanAppProject");
        }


        public async Task<(double lat, double lon)?> GetCoordinatesFromZipAsync(string zipCode)
        {
            try
            {
                var url = $"https://nominatim.openstreetmap.org/search?postalcode={zipCode}&country=Israel&format=json";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var root = json.RootElement;

                if (root.GetArrayLength() > 0)
                {
                    var firstResult = root[0];
                    double lat = double.Parse(firstResult.GetProperty("lat").GetString());
                    double lon = double.Parse(firstResult.GetProperty("lon").GetString());
                    return (lat, lon);
                }
            }
            catch { /* לוג שגיאה במקרה של בעיית תקשורת */ }

            return null;
        }

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // רדיוס כדור הארץ
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

