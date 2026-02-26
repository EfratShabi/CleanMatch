using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.Repositories
{
    public interface ILocationService
    {
        Task<(double lat, double lon)?> GetCoordinatesFromZipAsync(string zipCode);
        // הפונקציה שתחשב מרחק בין שתי נקודות double CalculateDistance(double lat1, double lon1, double lat2, double lon2);  
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    }
}
