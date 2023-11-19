using ApiGympass.Data.Dtos;
using ApiGympass.Models;

namespace Tests.TestData.Models
{
    public class UserTestData
    {
        public CreateUserDto CreateUserDto { get; set; }
        public User User { get; set; }
        public ReadUserDto ReadUserDto { get; set; }
        public LoginUserDto LoginUserDto { get; set; }
    }
}