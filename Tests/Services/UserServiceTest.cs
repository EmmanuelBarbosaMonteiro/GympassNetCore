using Moq;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.Extensions.Logging;
using ApiGympass.Services.Implementations;
using ApiGympass.Data.Dtos;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using Microsoft.Extensions.Options;
using Tests.TestData.Builders;
using Tests.TestData.Models;

namespace ApiGympass.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<ILogger<UserService>> _loggerMock = new Mock<ILogger<UserService>>();
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userManagerMock = CreateMockUserManager();
            _userService = new UserService(
                _userRepositoryMock.Object, 
                _userManagerMock.Object, 
                _mapperMock.Object, 
                _loggerMock.Object);
        }

        [Fact]
        public async Task CreateUserAsync_WithValidData_ShouldCreateUserSuccessfully()
        {
            // Arrange
            var testData = new UserDataBuilder().Build();
            SetupMocksForCreateUser(testData);

            // Act
            var (result, returnedReadUserDto) = await _userService.CreateUserAsync(testData.CreateUserDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(testData.ReadUserDto, returnedReadUserDto);
            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ReadUserDto>(It.IsAny<User>()), Times.Once);
        }

        private Mock<UserManager<User>> CreateMockUserManager()
        {
            return new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object, 
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[] { },
                new IPasswordValidator<User>[] { },
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);
        }

        private void SetupMocksForCreateUser(UserTestData testData)
        {
            _mapperMock.Setup(m => m.Map<User>(It.IsAny<CreateUserDto>())).Returns(testData.User);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);
            _mapperMock.Setup(m => m.Map<ReadUserDto>(It.IsAny<User>())).Returns(testData.ReadUserDto);
        }
    }
}
