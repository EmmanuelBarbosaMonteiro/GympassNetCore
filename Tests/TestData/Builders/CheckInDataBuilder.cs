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
        private ReadGymDto _readGymDto;

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

            _readGymDto = new ReadGymDto
            {
                Id = Guid.NewGuid(),
                Title = "Gym Name",
                Latitude = 0.0m,
                Longitude = 0.0m
            };
        }

        public CheckInDataBuilder WithCloseProximity()
        {
            _createCheckInDto.UserLatitude = -27.2100000m;
            _createCheckInDto.UserLongitude = -49.6500000m;

            _readGymDto.Latitude = _createCheckInDto.UserLatitude;
            _readGymDto.Longitude = _createCheckInDto.UserLongitude;

            return this;
        }

        public CheckInDataBuilder WithDistantProximity()
        {
            _createCheckInDto.UserLatitude = -27.2100000m;
            _createCheckInDto.UserLongitude = -49.6500000m;

            _readGymDto.Latitude = -27.2092052m;
            _readGymDto.Longitude = -49.6401091m;
            
            return this;
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