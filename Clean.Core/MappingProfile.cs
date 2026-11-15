using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Clean.Core
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Cleaners, CleanerDTO>().ReverseMap();
            CreateMap<Feedback, FeedbackDTO>().ReverseMap();
            CreateMap<Customers, CustomerDTO>().ReverseMap();
            CreateMap<Bookings, BookingDTO>()
                .ForMember(d => d.IsCompleted, o => o.MapFrom((src, dest, member, ctx) =>
                    src.Status == BookingStatus.Confirmed
                    && ctx.Items.TryGetValue("now", out var now)
                    && src.HasEnded((DateTime)now)));
            CreateMap<Availability, AvailabilityDTO>();
            CreateMap<BlockedDate, BlockedDateDTO>();

           
        }
    }
}
