using ApiGympass.Models;
using ApiGympass.Data.Dtos;
using Bogus;
using Tests.TestData.Models;

namespace Tests.TestData.Builders
{
    public class GymDataBuilder
    {
        private Gym _gym;
        private CreateGymDto _createGymDto;
        private ReadGymDto _readGymDto;
        private List<Gym> _gyms = new List<Gym>();

        public GymDataBuilder()
        {
            var faker = new Faker();

            _gym = new Gym
            {
                Id = Guid.NewGuid(),
                Title = faker.Company.CompanyName(),
                Latitude = (decimal?)faker.Address.Latitude(),
                Longitude = (decimal?)faker.Address.Longitude(),
                Description = faker.Lorem.Paragraph(),
                Phone = faker.Phone.PhoneNumber()
            };

            _createGymDto = new CreateGymDto
            {
                Title = _gym.Title,
                Latitude = _gym.Latitude,
                Longitude = _gym.Longitude,
                Description = _gym.Description,
                Phone = _gym.Phone
            };

            _readGymDto = new ReadGymDto
            {
                Id = _gym.Id,
                Title = _gym.Title,
                Latitude = _gym.Latitude,
                Longitude = _gym.Longitude,
                Description = _gym.Description,
                Phone = _gym.Phone
            };
        }

        public GymDataBuilder WithGymId(Guid id)
        {
            _gym.Id = id;
            _readGymDto.Id = id;
            return this;
        }

        public GymDataBuilder WithMultipleGyms(int count)
        {
            _gyms.Clear();
            var gymFaker = new Faker<Gym>()
                .RuleFor(g => g.Id, f => Guid.NewGuid())
                .RuleFor(g => g.Title, f => f.Company.CompanyName())
                .RuleFor(g => g.Latitude, (f, g) => (decimal?)f.Address.Latitude())
                .RuleFor(g => g.Longitude, (f, g) => (decimal?)f.Address.Longitude())
                .RuleFor(g => g.Description, f => f.Lorem.Paragraph())
                .RuleFor(g => g.Phone, f => f.Phone.PhoneNumber());

            for (int i = 0; i < count; i++)
            {
                _gyms.Add(gymFaker.Generate());
            }

            return this;
        }

        public Gym BuildGym()
        {
            return _gym;
        }

        public CreateGymDto BuildCreateGymDto()
        {
            return _createGymDto;
        }

        public ReadGymDto BuildReadGymDto()
        {
            return _readGymDto;
        }

        public List<Gym> BuildGymList()
        {
            return _gyms;
        }

        public GymTestData BuildGymTestData()
        {
            return new GymTestData
            {
                CreateGymDto = _createGymDto,
                ReadGymDto = _readGymDto
            };
        }
    }
}