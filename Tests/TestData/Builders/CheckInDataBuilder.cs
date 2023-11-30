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
        private List<CheckIn> _checkIns = new List<CheckIn>();
        private List<ReadCheckInDto> _readCheckInDtos = new List<ReadCheckInDto>();
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

        public CheckInDataBuilder WithMultipleCheckIns(int count)
        {
            _checkIns.Clear();

            var faker = new Faker<CheckIn>()
                .RuleFor(c => c.UserId, f => _checkIn.UserId)
                .RuleFor(c => c.GymId, f => _checkIn.GymId)
                .RuleFor(c => c.CreatedAt, f => DateTime.UtcNow);

            for (int i = 0; i < count; i++)
            {
                var checkIn = faker.Generate();
                _checkIns.Add(checkIn);
            }

            return this;
        }

        public List<CheckIn> GetCheckIns()
        {
            return _checkIns;
        }

        public CheckInDataBuilder WithUserId(Guid userId)
        {
            _createCheckInDto.UserId = userId;
            _checkIn.UserId = userId;
            _readCheckInDto.UserId = userId;

            if (_checkIns != null)
            {
                foreach (var checkIn in _checkIns)
                {
                    checkIn.UserId = userId;
                }
            }

            return this;
        }

        public CheckInDataBuilder WithValidCheckIn()
        {
            var currentTime = DateTime.UtcNow;
            var checkInId = Guid.NewGuid();
            
            _checkIn = new CheckIn
            {
                Id = checkInId,
                UserId = Guid.NewGuid(),
                GymId = Guid.NewGuid(),
                ValidateAt = null,
                CreatedAt = currentTime.AddMinutes(-10)
            };

            _readCheckInDto = new ReadCheckInDto(
                checkInId,
                _checkIn.UserId ?? Guid.Empty,
                _checkIn.GymId ?? Guid.Empty,
                _checkIn.ValidateAt,
                _checkIn.CreatedAt
            );

            return this;
        }

        public CheckInDataBuilder WithLateCheckIn()
        {
            var currentTime = DateTime.UtcNow;
            var checkInId = Guid.NewGuid();

            _checkIn = new CheckIn
            {
                Id = checkInId,
                UserId = Guid.NewGuid(),
                GymId = Guid.NewGuid(),
                ValidateAt = null,
                CreatedAt = currentTime.AddMinutes(-30)
            };

            _readCheckInDto = new ReadCheckInDto(
                checkInId,
                _checkIn.UserId ?? Guid.Empty,
                _checkIn.GymId ?? Guid.Empty,
                _checkIn.ValidateAt,
                _checkIn.CreatedAt
            );

            return this;
        }
    }
}