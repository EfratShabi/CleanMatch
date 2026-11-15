using AutoMapper;
using Clean.Core.Entities;
using CleanAPI.Models;


namespace CleanAPI
{
    public class MappingPostModel : Profile
    {
        public MappingPostModel()
        {
            CreateMap<CustomerPostModel, Customers>().ReverseMap();
            CreateMap<CustomerPutModel, Customers>();
            CreateMap<CleanerPostModel, Cleaners>().ReverseMap();
            CreateMap<CleanerPutModel, Cleaners>();
            CreateMap<BookingPostModel, Bookings>().ReverseMap();
             CreateMap<FeedbackPostModel, Feedback>().ReverseMap();
            CreateMap<AvailabilitySlotModel, Availability>();

        }

    }
}
