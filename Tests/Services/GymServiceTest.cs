using ApiGympass.Data.Dtos;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using ApiGympass.Services.ErrorHandling;
using ApiGympass.Services.Implementations;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.TestData.Builders;

namespace Tests.Services
{
    public class GymServiceTest
    {
        private readonly Mock<IGymRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<GymService>> _mockLogger;
        private readonly GymService _service;
        private readonly GymDataBuilder _dataBuilder;

        public GymServiceTest()
        {
            _mockRepository = new Mock<IGymRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<GymService>>();
            _service = new GymService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
            _dataBuilder = new GymDataBuilder();
        }

        [Fact]
        public async Task CreateGymAsync_WithValidData_ShouldCreateGymSuccessfully()
        {
            // Arrange
            var gymTestData = _dataBuilder.BuildGymTestData();

            _mockMapper.Setup(mapper => mapper.Map<Gym>(gymTestData.CreateGymDto))
                       .Returns(_dataBuilder.BuildGym());

            _mockRepository.Setup(repo => repo.CreateGymAsync(It.IsAny<Gym>()))
                           .Returns(Task.FromResult(_dataBuilder.BuildGym()));

            _mockMapper.Setup(mapper => mapper.Map<ReadGymDto>(It.IsAny<Gym>()))
                       .Returns(gymTestData.ReadGymDto);

            // Act
            var result = await _service.CreateGymAsync(gymTestData.CreateGymDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gymTestData.ReadGymDto.Id, result.Id);
        }

        [Fact]
        public async Task FindById_WithValidId_ShouldReturnGym()
        {
            // Arrange
            var gymId = Guid.NewGuid();
            var gymTestData = new GymDataBuilder()
                .WithGymId(gymId)
                .BuildGymTestData();

            _mockRepository.Setup(repo => repo.FindById(gymId))
                            .ReturnsAsync(gymTestData.Gym);

            _mockMapper.Setup(mapper => mapper.Map<ReadGymDto>(gymTestData.Gym))
                        .Returns(gymTestData.ReadGymDto);

            // Act
            var result = await _service.FindById(gymId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gymId, result.Id);
        }

        [Fact]
        public async Task SearchGymsAsync_WithValidQuery_ShouldReturnGymsAndHasNextPage()
        {
            // Arrange
            var gymDataBuilder = new GymDataBuilder().WithMultipleGyms(25);
            var gyms = gymDataBuilder.BuildGymList();
            var query = "test";
            var page = 1;
            var pageSize = 20;

            _mockRepository.Setup(repo => repo.SearchManyAsync(query, page, pageSize))
                            .ReturnsAsync(gyms.Take(pageSize));

            _mockMapper.Setup(mapper => mapper.Map<ReadGymDto>(It.IsAny<Gym>()))
                        .Returns<Gym>(gym => gymDataBuilder.WithGymId(gym.Id).BuildReadGymDto());

            _mockRepository.Setup(repo => repo.CountAsync(query))
                            .ReturnsAsync(25);

            // Act
            var (resultGyms, hasNextPage) = await _service.SearchGymsAsync(query, page);

            // Assert
            Assert.Equal(pageSize, resultGyms.Count());
            Assert.True(hasNextPage);
        }
    }
}