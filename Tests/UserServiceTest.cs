using Moq;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.Extensions.Logging;
using ApiGympass.Services.Implementations;
using ApiGympass.Data.Dtos;
using Bogus;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using Microsoft.Extensions.Options;

namespace ApiGympass.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userManagerMock = new Mock<UserManager<User>>(
                 new Mock<IUserStore<User>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object, 
                    new Mock<IPasswordHasher<User>>().Object,
                    new IUserValidator<User>[] { },
                    new IPasswordValidator<User>[] { },
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<IServiceProvider>().Object,
                    new Mock<ILogger<UserManager<User>>>().Object
            );
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<UserService>>();

            _userService = new UserService(
                _userRepositoryMock.Object, 
                _userManagerMock.Object, 
                _mapperMock.Object, 
                _loggerMock.Object);
        }

        [Fact]
        public async Task CreateUserAsync_WhenCalled_ShouldReturnIdentityResultAndReadUserDto()
        {
            // Arrange
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
                faker.Internet.Email(),
                faker.Internet.UserName()
            );

            _mapperMock.Setup(m => m.Map<User>(It.IsAny<CreateUserDto>())).Returns(user);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);
            _mapperMock.Setup(m => m.Map<ReadUserDto>(It.IsAny<User>())).Returns(readUserDto);

            // Act
            var (result, returnedReadUserDto) = await _userService.CreateUserAsync(createUserDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(readUserDto, returnedReadUserDto);

            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ReadUserDto>(It.IsAny<User>()), Times.Once);
        }
    }
}
