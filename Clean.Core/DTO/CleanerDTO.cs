using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.DTO
{
    public class CleanerDTO
    {
        //public int IdOfCleaner { get; set; }
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhonNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string DescriptionOnMe { get; set; }
        public bool BornIsrael { get; set; }
        public bool HebrewSpeaker { get; set; }
        public int PricePerHour { get; set; }
        public int UpToKm { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public List<AvailabilityDTO> ArrAvailability { get; set; }

        //public Schedule Schedule { get; set; }
    }
}
