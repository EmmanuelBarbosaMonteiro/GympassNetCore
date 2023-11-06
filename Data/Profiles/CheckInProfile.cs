using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using AutoMapper;

namespace ApiGympass.Data.Profiles
{
    public class CheckInProfile : Profile
    {
        public CheckInProfile()
        {
            CreateMap<CreateCheckInDto, CheckIn>();
        }
    }
}