using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using System.Collections.Generic;

namespace Tests.TestData.Models
{
    public class CheckInTestData
    {
        public CreateCheckInDto CreateCheckInDto { get; set; }
        public CheckIn CheckIn { get; set; }
        public ReadCheckInDto ReadCheckInDto { get; set; }
        public IEnumerable<CheckIn> CheckIns { get; set; }
    }
}