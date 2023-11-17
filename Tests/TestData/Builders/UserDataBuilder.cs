using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using Bogus;
using Tests.TestData.Models;

namespace Tests.TestData.Builders
{
    public class UserDataBuilder
    {
        private CreateUserDto _createUserDto;
        private User _user;
        private ReadUserDto _readUserDto;

        public UserDataBuilder()
        {
            var faker = new Faker();
            _createUserDto = new CreateUserDto 
            {
                UserName = faker.Name.FullName(),
                Email = faker.Internet.Email(),
                Password = faker.Internet.Password()
            };

            _user = new User 
            { 
                UserName = faker.Internet.UserName(),
                Email = faker.Internet.Email(),
                PasswordHash = faker.Internet.Password(),
                State = State.Active,
                CreatedAt = DateTime.UtcNow
            };

            _readUserDto = new ReadUserDto(
                Guid.NewGuid(),
                _user.Email,
                _user.UserName
            );
        }

        public UserDataBuilder WithEmail(string email)
        {
            _createUserDto.Email = email;
            _user.Email = email;
            _readUserDto.Email = email;
            return this;
        }

        public UserDataBuilder WithUserName(string userName)
        {
            _createUserDto.UserName = userName;
            _user.UserName = userName;
            _readUserDto.UserName = userName;
            return this;
        }

        public UserTestData Build()
        {
            return new UserTestData
            {
                CreateUserDto = _createUserDto,
                User = _user,
                ReadUserDto = _readUserDto
            };
        }
    }
}