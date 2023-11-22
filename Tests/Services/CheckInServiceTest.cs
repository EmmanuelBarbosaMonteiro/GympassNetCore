using ApiGympass.Data.Dtos;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using ApiGympass.Services.ErrorHandling;
using ApiGympass.Services.Implementations;
using ApiGympass.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.TestData.Builders;

namespace Tests.Services
{
    public class CheckInServiceTest
    {
        private readonly Mock<ICheckInRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IGymService> _mockGymService;
        private readonly Mock<ILogger<CheckInService>> _mockLogger;
        private readonly CheckInService _service;
        private readonly CheckInDataBuilder _dataBuilder;

        public CheckInServiceTest()
        {
            _mockRepository = new Mock<ICheckInRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUserService = new Mock<IUserService>();
            _mockGymService = new Mock<IGymService>();
            _mockLogger = new Mock<ILogger<CheckInService>>();
            _service = new CheckInService(_mockRepository.Object, _mockMapper.Object, _mockUserService.Object, _mockGymService.Object, _mockLogger.Object);
            _dataBuilder = new CheckInDataBuilder();
        }

        [Fact]
        public async Task CreateCheckInAsync_WithValidData_ShouldCreateCheckInSuccessfully()
        {
            // Arrange
            var checkInTestData = _dataBuilder.Build();
            var userDataBuilder = new UserDataBuilder();
            var userTestData = userDataBuilder.Build();

            _mockGymService.Setup(service => service.FindById(checkInTestData.CreateCheckInDto.GymId))
               .ReturnsAsync(new ReadGymDto());
            _mockUserService.Setup(service => service.GetByIdAsync(checkInTestData.CreateCheckInDto.UserId))
                            .ReturnsAsync(userTestData.ReadUserDto);
            _mockMapper.Setup(mapper => mapper.Map<CheckIn>(checkInTestData.CreateCheckInDto)).Returns(checkInTestData.CheckIn);
            _mockMapper.Setup(mapper => mapper.Map<ReadCheckInDto>(checkInTestData.CheckIn)).Returns(checkInTestData.ReadCheckInDto);

            // Act
            var result = await _service.CreateCheckInAsync(checkInTestData.CreateCheckInDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(checkInTestData.ReadCheckInDto.UserId, result.UserId);
        }

        [Fact]
        public async Task CreateCheckInAsync_WithNonExistingGym_ShouldFail()
        {
            // Arrange
            var checkInTestData = _dataBuilder.Build();
            var userDataBuilder = new UserDataBuilder();
            var userTestData = userDataBuilder.Build();

            _mockUserService.Setup(service => service.GetByIdAsync(checkInTestData.CreateCheckInDto.UserId))
                            .ReturnsAsync(userTestData.ReadUserDto);
            _mockGymService.Setup(service => service.FindById(checkInTestData.CreateCheckInDto.GymId))
                            .ReturnsAsync((ReadGymDto)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<GymNotFoundError>(() => _service.CreateCheckInAsync(checkInTestData.CreateCheckInDto));

            Assert.IsType<GymNotFoundError>(exception);
        }

        [Fact]
        public async Task GetCheckInByIdAsync_WithExistingId_ShouldReturnCheckIn()
        {
            // Arrange
            var checkInTestData = _dataBuilder.Build();
            _mockRepository.Setup(repo => repo.FindById(checkInTestData.CheckIn.Id))
                        .ReturnsAsync(checkInTestData.CheckIn);
            _mockMapper.Setup(mapper => mapper.Map<ReadCheckInDto>(checkInTestData.CheckIn))
                    .Returns(checkInTestData.ReadCheckInDto);

            // Act
            var result = await _service.GetCheckInByIdAsync(checkInTestData.CheckIn.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(checkInTestData.ReadCheckInDto.UserId, result.UserId);
        }

        [Fact]
        public async Task CreateCheckInAsync_TwiceOnSameDay_ShouldFail()
        {
            // Arrange
            var checkInDate = DateTime.UtcNow;
            var checkInTestData = _dataBuilder.WithCheckInDate(checkInDate).Build();
            var userTestData = new UserDataBuilder().Build();

            _mockGymService.Setup(service => service.FindById(checkInTestData.CreateCheckInDto.GymId))
               .ReturnsAsync(new ReadGymDto());
            _mockUserService.Setup(service => service.GetByIdAsync(checkInTestData.CreateCheckInDto.UserId))
                            .ReturnsAsync(userTestData.ReadUserDto);
            _mockMapper.Setup(mapper => mapper.Map<CheckIn>(It.IsAny<CreateCheckInDto>()))
                    .Returns<CreateCheckInDto>(dto => new CheckIn { UserId = dto.UserId, GymId = dto.GymId, CreatedAt = checkInDate });
            _mockMapper.Setup(mapper => mapper.Map<ReadCheckInDto>(It.IsAny<CheckIn>()))
                    .Returns<CheckIn>(checkIn => new ReadCheckInDto(checkIn.Id, checkIn.UserId ?? Guid.Empty, checkIn.GymId ?? Guid.Empty, checkIn.ValidateAt, checkIn.CreatedAt));
            
            _mockRepository.Setup(repo => repo.FindByUserIdOnDate(checkInTestData.CreateCheckInDto.UserId, checkInDate))
                        .ReturnsAsync(new CheckIn());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CheckInLimitExceeded>(() => _service.CreateCheckInAsync(checkInTestData.CreateCheckInDto));
        }

        [Fact]
        public async Task CreateCheckInAsync_OnDifferentDays_ShouldSucceed()
        {
            // Arrange
            var firstDay = DateTime.UtcNow;
            var secondDay = firstDay.AddDays(1);

            var checkInTestDataDay1 = new CheckInDataBuilder().WithCheckInDate(firstDay).Build();
            var checkInTestDataDay2 = new CheckInDataBuilder().WithCheckInDate(secondDay).Build();
            var userTestData = new UserDataBuilder().Build();

            _mockGymService.Setup(service => service.FindById(It.IsAny<Guid>()))
               .ReturnsAsync(new ReadGymDto());
            _mockUserService.Setup(service => service.GetByIdAsync(It.IsAny<Guid>()))
                            .ReturnsAsync(userTestData.ReadUserDto);
            _mockMapper.Setup(mapper => mapper.Map<CheckIn>(It.IsAny<CreateCheckInDto>()))
                    .Returns<CreateCheckInDto>(dto => new CheckIn { UserId = dto.UserId, GymId = dto.GymId, CreatedAt = dto.CreatedAt });
            _mockMapper.Setup(mapper => mapper.Map<ReadCheckInDto>(It.IsAny<CheckIn>()))
                    .Returns<CheckIn>(checkIn => new ReadCheckInDto(checkIn.Id, checkIn.UserId ?? Guid.Empty, checkIn.GymId ?? Guid.Empty, checkIn.ValidateAt, checkIn.CreatedAt));

            var firstCheckInResult = await _service.CreateCheckInAsync(checkInTestDataDay1.CreateCheckInDto);

            _mockRepository.Setup(repo => repo.FindByUserIdOnDate(checkInTestDataDay2.CreateCheckInDto.UserId, secondDay))
                        .ReturnsAsync((CheckIn)null);

            // Act
            var secondCheckInResult = await _service.CreateCheckInAsync(checkInTestDataDay2.CreateCheckInDto);

            // Assert
            Assert.NotNull(secondCheckInResult);
            Assert.Equal(checkInTestDataDay2.ReadCheckInDto.UserId, secondCheckInResult.UserId);
            Assert.NotEqual(firstCheckInResult.CreatedAt, secondCheckInResult.CreatedAt);
        }
    }

}