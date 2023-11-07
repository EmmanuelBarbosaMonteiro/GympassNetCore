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

            CreateMap<User, ReadUserDto>()
            .ConstructUsing(user => new ReadUserDto(user.Id, user.Email, user.Name));
        }
    }
}