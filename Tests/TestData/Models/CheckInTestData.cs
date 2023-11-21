using ApiGympass.Data.Dtos;
using ApiGympass.Models;

namespace Tests.TestData.Models
{
    public class CheckInTestData
    {
        public CreateCheckInDto CreateCheckInDto { get; set; }
        public CheckIn CheckIn { get; set; }
        public ReadCheckInDto ReadCheckInDto { get; set; }
    }
}