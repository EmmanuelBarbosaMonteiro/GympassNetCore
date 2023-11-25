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
            var checkIns = new Faker<CheckIn>()
                .RuleFor(c => c.UserId, f => _checkIn.UserId)
                .RuleFor(c => c.GymId, f => _checkIn.GymId)
                .RuleFor(c => c.CreatedAt, f => DateTime.UtcNow)
                .Generate(count);

           _readCheckInDtos = new Faker<CheckIn>()
                .RuleFor(c => c.UserId, _checkIn.UserId)
                .RuleFor(c => c.GymId, f => _checkIn.GymId)
                .RuleFor(c => c.CreatedAt, f => DateTime.UtcNow)
                .Generate(count)
                .Select(c => new ReadCheckInDto(
                    Guid.NewGuid(),
                    c.UserId ?? Guid.Empty,
                    c.GymId ?? Guid.Empty,
                    c.ValidateAt,
                    c.CreatedAt
                )).ToList();

            return this;
        }

        public CheckInDataBuilder WithUserId(Guid userId)
        {
            _createCheckInDto.UserId = userId;
            _checkIn.UserId = userId;
            _readCheckInDto.UserId = userId;
           
            if (_readCheckInDtos != null)
            {
                _readCheckInDtos.ForEach(dto => dto.UserId = userId);
            }

            return this;
        }

        public List<ReadCheckInDto> BuildReadCheckInDtoList(int count)
        {
            return Enumerable.Range(0, count).Select(_ => new ReadCheckInDto(
                Guid.NewGuid(),
                _checkIn.UserId ?? Guid.Empty,
                _checkIn.GymId ?? Guid.Empty,
                _checkIn.ValidateAt,
                DateTime.UtcNow
            )).ToList();
        }
    }
}