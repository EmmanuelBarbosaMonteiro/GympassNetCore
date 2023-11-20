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
using Tests.TestData.Models;

namespace Tests.Services
{
    public class CheckInServiceTest
    {
        private readonly Mock<ICheckInRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<CheckInService>> _mockLogger;
        private readonly CheckInService _service;
        private readonly CheckInDataBuilder _dataBuilder;

        public CheckInServiceTest()
        {
            _mockRepository = new Mock<ICheckInRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<CheckInService>>();
            _service = new CheckInService(_mockRepository.Object, _mockMapper.Object, _mockUserService.Object, _mockLogger.Object);
            _dataBuilder = new CheckInDataBuilder();
        }

        [Fact]
        public async Task CreateCheckInAsync_WithValidData_ShouldCreateCheckInSuccessfully()
        {
            // Arrange
            var checkInTestData = _dataBuilder.Build();
            var userDataBuilder = new UserDataBuilder();
            var userTestData = userDataBuilder.Build();

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
        public async Task GetCheckInByIdAsync_WithExistingId_ShouldReturnCheckIn()
        {
            // Arrange
            var checkInTestData = _dataBuilder.Build();
            _mockRepository.Setup(repo => repo.GetCheckInByIdAsync(checkInTestData.CheckIn.Id))
                        .ReturnsAsync(checkInTestData.CheckIn);
            _mockMapper.Setup(mapper => mapper.Map<ReadCheckInDto>(checkInTestData.CheckIn))
                    .Returns(checkInTestData.ReadCheckInDto);

            // Act
            var result = await _service.GetCheckInByIdAsync(checkInTestData.CheckIn.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(checkInTestData.ReadCheckInDto.UserId, result.UserId);
        }
    }

}