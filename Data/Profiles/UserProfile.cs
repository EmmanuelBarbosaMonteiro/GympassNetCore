using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using AutoMapper;

namespace ApiGympass.Data.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, User>();

            CreateMap<UpdateUserDto, User>()
            .ReverseMap();

            CreateMap<User, ReadUserDto>()
            .ConstructUsing(user => new ReadUserDto(user.Id, user.Email, user.UserName));
        }
    }
}