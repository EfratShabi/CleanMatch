using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.DTO
{
    public class FeedbackDTO
    {
        public string TextOfFeedback { get; set; }
        public int CleanerId { get; set; }
        public Cleaners Cleaners { get; set; }
    }
}
