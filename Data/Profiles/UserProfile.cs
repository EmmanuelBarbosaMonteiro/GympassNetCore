using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using AutoMapper;

namespace ApiGympass.Data.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, User>()
            .ForMember(u => u.UserName, opt => opt.MapFrom(dto => dto.Name));

            CreateMap<UpdateUserDto, User>()
            .ForMember(u => u.UserName, opt => opt.MapFrom(dto => dto.Name))
            .ReverseMap();
        }
    }
}