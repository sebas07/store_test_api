using API.Dtos;
using AutoMapper;
using Core.Entities;

namespace API.Profiles
{
    public class MappingProfiles : Profile
    {

        public MappingProfiles()
        {
            CreateMap<Brand, BrandDto>()
                .ReverseMap();

            CreateMap<Category, CategoryDto>()
                .ReverseMap();
        }

    }
}
