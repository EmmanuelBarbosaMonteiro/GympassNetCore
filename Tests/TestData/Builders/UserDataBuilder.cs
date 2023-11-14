using ApiGympass.Data.Dtos;
using ApiGympass.Models;
using Bogus;
using Tests.TestData.Models;

namespace Tests.TestData.Builders
{
    public class UserDataBuilder
    {
        public UserTestData Build()
        {
            var faker = new Faker();
            var createUserDto = new CreateUserDto 
            {
                UserName = faker.Name.FullName(),
                Email = faker.Internet.Email(),
                Password = faker.Internet.Password()
            };

            var user = new User 
            { 
                UserName = faker.Internet.UserName(),
                Email = faker.Internet.Email(),
                PasswordHash = faker.Internet.Password(),
                State = State.Active,
                CreatedAt = DateTime.UtcNow
            };

            var readUserDto = new ReadUserDto(
                Guid.NewGuid(),
                user.Email,
                user.UserName
            );

            return new UserTestData
            {
                CreateUserDto = createUserDto,
                User = user,
                ReadUserDto = readUserDto
            };
        }
    }
}