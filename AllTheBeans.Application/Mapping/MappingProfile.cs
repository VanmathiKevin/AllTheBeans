
using AllTheBeans.Application.DTOs;
using AllTheBeans.Domain.Models;
using AutoMapper;

namespace AllTheBeans.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CoffeeBean, CoffeeBeanDto>().ReverseMap();
            CreateMap<CreateCoffeeBeanDto, CoffeeBean>();
        }
    }
}
