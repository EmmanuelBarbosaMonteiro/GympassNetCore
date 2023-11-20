using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using Bogus;
using Tests.TestData.Models;

namespace Tests.TestData.Builders
{
    public class CheckInDataBuilder
    {
        private CreateCheckInDto _createCheckInDto;
        private CheckIn _checkIn;
        private ReadCheckInDto _readCheckInDto;

        public CheckInDataBuilder()
        {
            var faker = new Faker();
            _createCheckInDto = new CreateCheckInDto
            {
                UserId = Guid.NewGuid(),
                GymId = Guid.NewGuid()
            };

            _checkIn = new CheckIn
            {
                UserId = Guid.NewGuid(),
                GymId = Guid.NewGuid(),
                ValidateAt = null,
                CreatedAt = DateTime.UtcNow
            };

            _readCheckInDto = new ReadCheckInDto(
                Guid.NewGuid(),
                _checkIn.UserId ?? Guid.NewGuid(),
                _checkIn.GymId ?? Guid.NewGuid(),
                _checkIn.ValidateAt,
                _checkIn.CreatedAt
            );
        }

        public CheckInTestData Build()
        {
            return new CheckInTestData
            {
                CreateCheckInDto = _createCheckInDto,
                CheckIn = _checkIn,
                ReadCheckInDto = _readCheckInDto
            };
        }
    }
}