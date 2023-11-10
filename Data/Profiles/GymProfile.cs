using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using AutoMapper;

namespace ApiGympass.Data.Profiles
{
    public class GymProfile : Profile
    {
        public GymProfile()
        {
            CreateMap<CreateGymDto, Gym>();
            CreateMap<Gym, ReadGymDto>();
        }
    }
}