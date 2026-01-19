using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.DTO
{
    public class BookingOfCleanerDTO
    {
        public double TotalPrice { get; set; }

        public DateTime Date { get; set; }

        public TimeOnly StartWork { get; set; }

        public TimeOnly EndWork { get; set; }


        public int cleanerId { get; set; }
    }
}
