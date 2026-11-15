using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Entities;
using Clean.Core.Models;
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
            CreateMap<Cleaners, CleanerDTO>()
                .ForMember(d => d.ReviewCount, o => o.MapFrom(s => s.ArrFidback == null ? 0 : s.ArrFidback.Count))
                .ForMember(d => d.AverageRating, o => o.MapFrom(s =>
                    s.ArrFidback == null || s.ArrFidback.Count == 0
                        ? (double?)null
                        : Math.Round(s.ArrFidback.Average(f => f.Rating), 1)));
            CreateMap<Feedback, FeedbackDTO>();
            CreateMap<CleanerMatch, CleanerSearchResultDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Cleaner.Id))
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.Cleaner.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.Cleaner.LastName))
                .ForMember(d => d.City, o => o.MapFrom(s => s.Cleaner.City))
                .ForMember(d => d.DescriptionOnMe, o => o.MapFrom(s => s.Cleaner.DescriptionOnMe))
                .ForMember(d => d.BornIsrael, o => o.MapFrom(s => s.Cleaner.BornIsrael))
                .ForMember(d => d.HebrewSpeaker, o => o.MapFrom(s => s.Cleaner.HebrewSpeaker))
                .ForMember(d => d.PricePerHour, o => o.MapFrom(s => s.Cleaner.PricePerHour))
                .ForMember(d => d.ReviewCount, o => o.MapFrom(s => s.Cleaner.ArrFidback == null ? 0 : s.Cleaner.ArrFidback.Count))
                .ForMember(d => d.AverageRating, o => o.MapFrom(s =>
                    s.Cleaner.ArrFidback == null || s.Cleaner.ArrFidback.Count == 0
                        ? (double?)null
                        : Math.Round(s.Cleaner.ArrFidback.Average(f => f.Rating), 1)))
                .ForMember(d => d.DistanceKm, o => o.MapFrom(s => Math.Round(s.DistanceKm, 1)))
                .ForMember(d => d.MatchScore, o => o.MapFrom(s => s.Score));
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
