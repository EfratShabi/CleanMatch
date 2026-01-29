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
            CreateMap<CleanerPostModel, Cleaners>().ReverseMap();
        }

    }
}
