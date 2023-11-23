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
            var userId = Guid.NewGuid();
            var gymId = Guid.NewGuid();

            _createCheckInDto = new CreateCheckInDto
            {
                UserId = userId,
                UserLatitude = -27.2092052m,
                UserLongitude = -49.6401091m,
                GymId = gymId,
            };

            _checkIn = new CheckIn
            {
                UserId = userId,
                GymId = gymId,
                ValidateAt = null,
                CreatedAt = DateTime.UtcNow
            };

            _readCheckInDto = new ReadCheckInDto(
                Guid.NewGuid(),
                userId,
                gymId,
                _checkIn.ValidateAt,
                _checkIn.CreatedAt
            );
        }

         public CheckInDataBuilder WithCheckInDate(DateTime date)
        {
            _checkIn.CreatedAt = date;
            return this;
        }

        public CheckInTestData BuildForSameDay()
        {
            var date = DateTime.UtcNow;
            _checkIn.CreatedAt = date;
            return Build();
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