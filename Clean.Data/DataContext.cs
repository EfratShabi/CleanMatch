
using Clean.Core.Entities;

namespace Clean.Data
{

    public class DataContext
    {
        public List<Cleaners> Cleaners { get; set; }
        public List<Customers> Customers { get; set; }
        public List<Bookings> Bookings { get; set; }
        public List<Schedule> Schedule { get; set; }

        public DataContext()
        {
            Cleaners = new List<Cleaners>
            {
                new Cleaners
                {
                    Id = 1,
                    IdOfCleaner = 1212,
                    FirstName = "shiri",
                    LastName = "choen",
                    PhonNumber = "054444",
                    CodeCity = 2,
                    DescriptionOnMe = "i lnhdfjghf",
                    BornIsrael = true,
                    availability = false,
                    HebrewSpeaker = true,
                    Mikud = "1234567",
                    PricePerHour = 60,
                    UpToKm = 7,
                    ArrFidback = new List<string> { "terre", "trgfbf" }, 
                    
                    // --- תיקון #1: עטיפת ה-Bookings ברשימה חדשה ---
                    ArrBookings = new List<Bookings>
                    {
                        new Bookings
                        {
                            BookingCode = 1,
                            TotalPrice = 40,
                            CodeCustomer = 12,
                            CodeCleaner = 123,
                            Date = new DateTime(2025, 10, 26),
                            StartWork = new TimeOnly(11, 30, 0),
                            EndWork = new TimeOnly(13, 30, 0)
                        }
                    }
                }
            };

            Customers = new List<Customers>
            {
                new Customers
                {
                    Id = 1,
                    IdOfCustomer = 12345,
                    FirstName = "ee",
                    LastName = "ww",
                    PhonNumber = "0488888",
                    CodeCity = 3,
                    Mikud = "3456788",
                    NumberFlat = 12,
                    CodeStreet = 111
                }
            };

            Bookings = new List<Bookings>
            {
                new Bookings
                {
                    BookingCode = 1,
                    TotalPrice = 40,
                    CodeCustomer = 12,
                    CodeCleaner = 123,
                    Date = new DateTime(2025, 10, 26),
                    StartWork = new TimeOnly(11, 30, 0),
                    EndWork = new TimeOnly(13, 30, 0)
                }
            };

            
            Schedule = new List<Schedule>
            {
                new Schedule
                {
                    StartWork = new List<TimeOnly> { new TimeOnly(11, 30, 0), new TimeOnly(16, 30, 0) },
                    EndWork = new List<TimeOnly> { new TimeOnly(13, 30, 0), new TimeOnly(20, 30, 0) }
                }
            };
        }
    }
}