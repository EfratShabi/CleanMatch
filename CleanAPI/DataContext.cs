using CleanAPI.Entities;

namespace CleanAPI
{
    public class DataContext:IDataContext
    {
        public List<Cleaners> Cleaners { get; set; }
        public List<Customers> Customers { get; set; }
        public List<Bookings> Bookings { get; set; }
        public List<Schedule> schedules { get; set; }
        public DataContext()
        {
            Cleaners = new List<Cleaners>{ new Cleaners {Id=1,IdOfCleaner="1212" ,FirstName="shiri",LastName="choen",PhonNumber="054444",CodeCity=2,
                DescriptionOnMe ="i lnhdfjghf",BornIsrael=true,availability=false,HebrewSpeaker=true,Mikud="1234567",PricePerHour=60,ArrFidback=["terre","trgfbf"]} };

            Customers = new List<Customers> { new Customers { Id = 1, IdOfCustomer = "12345", FirstName = "ee", LastName = "ww", PhonNumber = "0488888", CodeCity = 3, Mikud = "3456788" } };

            Bookings = new List<Bookings> { new Bookings { BookingCode = 1, IdOfCleaner = "1212", IdOfCustomer = "3456", TotalPrice = 40,
              StartWork=  new TimeOnly(11, 30, 0), EndWork =  new TimeOnly(13, 30, 0) } };

            schedules = new List<Schedule> { new Schedule { StartWork = { new TimeOnly(11, 30, 0), new TimeOnly(16, 30, 0) }, EndWork = { new TimeOnly(13, 30, 0), new TimeOnly(20, 30, 0) } } };
        }
    }
}