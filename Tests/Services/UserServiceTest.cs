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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using ApiGympass.Services.ErrorHandling;
using Project.Services.ErrorHandling;
using ApiGympass.Services.Interfaces;

namespace ApiGympass.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<ILogger<UserService>> _loggerMock = new Mock<ILogger<UserService>>();
        private readonly UserService _userService;
        private readonly Mock<TokenService> _tokenServiceMock = new Mock<TokenService>();
        private readonly Mock<IUserService> _decoratedUserServiceMock = new Mock<IUserService>();
        private readonly ArchivingUserService _archivingUserService;

        public UserServiceTests()
        {
            _userManagerMock = CreateMockUserManager();
            _signInManagerMock = CreateMockSignInManager();
            _userService = new UserService(
                _userRepositoryMock.Object, 
                _userManagerMock.Object, 
                _mapperMock.Object, 
                _loggerMock.Object,
                _signInManagerMock.Object,
                _tokenServiceMock.Object);

            _archivingUserService = new ArchivingUserService(
                _decoratedUserServiceMock.Object, 
                _userManagerMock.Object, 
                new Mock<ILogger<ArchivingUserService>>().Object);
        }

        [Fact]
        public async Task CreateUserAsync_WithValidData_ShouldCreateUserSuccessfully()
        {
            // Arrange
            var testData = new UserDataBuilder().Build();
            SetupMocksForCreateUser(testData);

            // Act
            var returnedReadUserDto = await _userService.CreateUserAsync(testData.CreateUserDto);

            // Assert
            Assert.Equal(testData.ReadUserDto, returnedReadUserDto);
            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ReadUserDto>(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_WithEmailAlreadyExists_ShouldThrowUserAlreadyExistsError()
        {
            // Arrange
            var testData = new UserDataBuilder().WithEmail("existingemail@example.com").Build();
            SetupMocksForEmailAlreadyExists(testData);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserAlreadyExistsError>(() => 
                _userService.CreateUserAsync(testData.CreateUserDto));

            Assert.NotNull(exception);
            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task LoginUserAsync_WithValidCredentials_ShouldReturnValidToken()
        {
            // Arrange
            var testData = new UserDataBuilder().Build();

            _userManagerMock.Setup(um => um.FindByEmailAsync(testData.LoginUserDto.Email)).ReturnsAsync(testData.User);
            _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(testData.User.UserName, testData.LoginUserDto.Password, false, false))
                            .ReturnsAsync(SignInResult.Success);

            // Act
            var token = await _userService.LoginUserAsync(testData.LoginUserDto);

            // Assert
            Assert.NotNull(token);
        }

        [Fact]
        public async Task LoginUserAsync_WithInvalidCredentials_ShouldThrowInvalidCredentialsPasswordError()
        {
            // Arrange
            var testData = new UserDataBuilder().Build();
            var  invalidLoginUserDto = new LoginUserDto { Email = testData.LoginUserDto.Email, Password = "wrongPassword" };

            _userManagerMock.Setup(um => um.FindByEmailAsync(testData.LoginUserDto.Email)).ReturnsAsync(testData.User);
            _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(testData.User.UserName, invalidLoginUserDto.Password, false, false))
                            .ReturnsAsync(SignInResult.Failed);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialsError>(() => _userService.LoginUserAsync(invalidLoginUserDto));
        }

        [Fact]
        public async Task LoginUserAsync_WithInvalidCredentials_ShouldThrowInvalidCredentialsEmailError()
        {
            // Arrange
            var testData = new UserDataBuilder().Build();
            var  invalidLoginUserDto = new LoginUserDto { Email = "invalidemail@exemple.com", Password = "wrongPassword" };

            _userManagerMock.Setup(um => um.FindByEmailAsync(testData.LoginUserDto.Email)).ReturnsAsync(testData.User);
            _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(invalidLoginUserDto.Email, invalidLoginUserDto.Password, false, false))
                            .ReturnsAsync(SignInResult.Failed);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialsError>(() => _archivingUserService.LoginUserAsync(invalidLoginUserDto));
        }

        [Fact]
        public async Task GetByIdAsync_WithValidUserId_ShouldReturnReadUserDto()
        {
            // Arrange
            var userDataBuilder = new UserDataBuilder();
            var testData = userDataBuilder.Build();
            var userId = testData.User.Id;

            _userRepositoryMock.Setup(repo => repo.FindById(userId)).ReturnsAsync(testData.User);
            _mapperMock.Setup(mapper => mapper.Map<ReadUserDto>(testData.User)).Returns(testData.ReadUserDto);

            // Act
            var returnedReadUserDto = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(returnedReadUserDto);
            Assert.Equal(testData.ReadUserDto, returnedReadUserDto);
            _userRepositoryMock.Verify(repo => repo.FindById(userId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ReadUserDto>(testData.User), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentUserId_ShouldReturnNullOrThrowNotFoundException()
        {
            // Arrange
             var nonExistentUserId = Guid.NewGuid();
            _userManagerMock.Setup(um => um.FindByIdAsync(nonExistentUserId.ToString())).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundError>(() => _archivingUserService.GetByIdAsync(nonExistentUserId));
            _userManagerMock.Verify(um => um.FindByIdAsync(nonExistentUserId.ToString()), Times.Once);
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

         private Mock<SignInManager<User>> CreateMockSignInManager()
        {
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();

            return new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                contextAccessorMock.Object,
                userPrincipalFactoryMock.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<User>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                null);
        }
        
        private void SetupMocksForCreateUser(UserTestData testData)
        {
            _mapperMock.Setup(m => m.Map<User>(It.IsAny<CreateUserDto>())).Returns(testData.User);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);
            _mapperMock.Setup(m => m.Map<ReadUserDto>(It.IsAny<User>())).Returns(testData.ReadUserDto);
        }

        private void SetupMocksForEmailAlreadyExists(UserTestData testData)
        {
            _userManagerMock.Setup(um => um.FindByEmailAsync(testData.CreateUserDto.Email))
                            .ReturnsAsync(testData.User);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email already exists" }));
        }
    }
}
