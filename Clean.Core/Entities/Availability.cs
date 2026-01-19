using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.Entities
{
    public class Availability
    {
        public int Id { get; set; }
        public int CleanerId { get; set; }
        public Cleaners Cleaner { get; set; }
        int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

    }
}
