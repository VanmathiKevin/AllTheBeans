
using AllTheBeans.Application.DTOs;
using AllTheBeans.Application.Exceptions;
using AllTheBeans.Application.Interfaces;
using AllTheBeans.Application.Services;
using AllTheBeans.Domain.Models;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AllTheBeans.Tests.Services
{
    public class CoffeeBeanServiceTests
    {
        private readonly Mock<ICoffeeBeanRepository> _mockCoffeeBeanRepository = new();
        private readonly Mock<IBeanOfTheDayRepository> _mockBeanOfTheDayRepository = new();
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IBeanSelectionStrategy> _mockBeanSelectionStrategy = new();
        private readonly Mock<IMapper> _mockMapper = new();
        private readonly Mock<ILogger<CoffeeBeanService>> _mockLogger = new();

        private readonly CoffeeBeanService _service;

        public CoffeeBeanServiceTests()
        {
            _service = new CoffeeBeanService(
                _mockCoffeeBeanRepository.Object,
                _mockBeanOfTheDayRepository.Object,
                _mockBeanSelectionStrategy.Object,
                _mockMapper.Object,
                _mockUnitOfWork.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllBeansAsync_ReturnsMappedDtos()
        {
            // Arrange
            var mockBeans = new List<CoffeeBean>
            {
                new() { Id = 1, Name = "FUTURIS" },
                new() { Id = 2, Name = "ZANITY" }
            };

            var mockDtos = new List<CoffeeBeanDto>
            {
                new() { Id = 1, Name = "FUTURIS" },
                new() { Id = 2, Name = "ZANITY" }
            };

            _mockCoffeeBeanRepository.Setup(repo => repo.GetAllBeansAsync())
                               .ReturnsAsync(mockBeans);

            _mockMapper.Setup(m => m.Map<IEnumerable<CoffeeBeanDto>>(mockBeans))
                       .Returns(mockDtos);

            // Act
            var result = await _service.GetAllBeansAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(mockDtos);

            _mockCoffeeBeanRepository.Verify(repo => repo.GetAllBeansAsync(), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map<IEnumerable<CoffeeBeanDto>>(mockBeans), Times.Once);
        }

        [Fact]
        public async Task GetBeanByIdAsync_ExistingId_ReturnsMappedDto()
        {
            // Arrange
            var mockBean = new CoffeeBean { Id = 1, Name = "FUTURIS" };
            var mockDto = new CoffeeBeanDto { Id = 1, Name = "FUTURIS" };

            _mockCoffeeBeanRepository.Setup(r => r.GetBeanByIdAsync(mockBean.Id))
                               .ReturnsAsync(mockBean);

            _mockMapper.Setup(m => m.Map<CoffeeBeanDto>(mockBean))
                       .Returns(mockDto);

            // Act
            var result = await _service.GetBeanByIdAsync(mockBean.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(mockDto);

            _mockCoffeeBeanRepository.Verify(r => r.GetBeanByIdAsync(mockBean.Id), Times.Once);
            _mockMapper.Verify(m => m.Map<CoffeeBeanDto>(mockBean), Times.Once);
        }

        [Fact]
        public async Task GetBeanByIdAsync_NonExistingId_ThrowsNotFoundException()
        {
            // Arrange
            int inavlidId = 999;

            _mockCoffeeBeanRepository.Setup(r => r.GetBeanByIdAsync(inavlidId))
                               .ReturnsAsync((CoffeeBean?)null);

            // Act
            Func<Task> act = async () => await _service.GetBeanByIdAsync(inavlidId);

            // Assert
            await act.Should()
                     .ThrowAsync<NotFoundException>()
                     .WithMessage($"Coffee bean with ID {inavlidId} not found.");

            _mockCoffeeBeanRepository.Verify(r => r.GetBeanByIdAsync(inavlidId), Times.Once);
            _mockMapper.Verify(m => m.Map<CoffeeBeanDto>(It.IsAny<CoffeeBean>()), Times.Never);
        }

        [Fact]
        public async Task CreateBeanAsync_ValidDto_ReturnsMappedDto()
        {
            // Arrange
            var mockCreateDto = new CreateCoffeeBeanDto { Name = "FUTURIS", Price = 10 };
            var mockBean = new CoffeeBean { Id = 1, Name = "FUTURIS", Price = 10 };
            var mockBeanDto = new CoffeeBeanDto { Id = 1, Name = "FUTURIS", Price = 10 };

            _mockMapper.Setup(m => m.Map<CoffeeBean>(mockCreateDto)).Returns(mockBean);
            _mockMapper.Setup(m => m.Map<CoffeeBeanDto>(mockBean)).Returns(mockBeanDto);

            _mockCoffeeBeanRepository.Setup(r => r.AddBeanAsync(mockBean)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateBeanAsync(mockCreateDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(mockBeanDto);

            _mockMapper.Verify(m => m.Map<CoffeeBean>(mockCreateDto), Times.Once);
            _mockCoffeeBeanRepository.Verify(r => r.AddBeanAsync(mockBean), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<CoffeeBeanDto>(mockBean), Times.Once);
        }

        [Fact]
        public async Task CreateBeanAsync_WhenSaveFails_ThrowsException()
        {
            // Arrange
            var mockCreateDto = new CreateCoffeeBeanDto { Name = "FUTURIS" };
            var mockBean = new CoffeeBean { Name = "FUTURIS" };

            _mockMapper.Setup(m => m.Map<CoffeeBean>(mockCreateDto)).Returns(mockBean);
            _mockCoffeeBeanRepository.Setup(r => r.AddBeanAsync(mockBean)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new Exception("DB failure"));

            // Act
            Func<Task> act = async () => await _service.CreateBeanAsync(mockCreateDto);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("DB failure");

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to create coffee bean")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateBeanAsync_ValidId_UpdatesAndReturnsTrue()
        {
            // Arrange
            int id = 1;
            var mockDto = new CreateCoffeeBeanDto { Name = "FUTURIS", Price = 12 };
            var mockBean = new CoffeeBean { Id = id, Name = "ZANITY", Price = 10 };

            _mockCoffeeBeanRepository.Setup(r => r.GetBeanByIdAsync(id)).ReturnsAsync(mockBean);
            _mockMapper.Setup(m => m.Map(mockDto, mockBean)); // Void return
            _mockCoffeeBeanRepository.Setup(r => r.UpdateBeanAsync(mockBean)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateBeanAsync(id, mockDto);

            // Assert
            result.Should().BeTrue();

            _mockCoffeeBeanRepository.Verify(r => r.GetBeanByIdAsync(id), Times.Once);
            _mockMapper.Verify(m => m.Map(mockDto, mockBean), Times.Once);
            _mockCoffeeBeanRepository.Verify(r => r.UpdateBeanAsync(mockBean), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateBeanAsync_NonExistingId_ThrowsNotFoundException()
        {
            // Arrange
            int id = 999;
            var mockDto = new CreateCoffeeBeanDto { Name = "FUTURI" };

            _mockCoffeeBeanRepository.Setup(r => r.GetBeanByIdAsync(id)).ReturnsAsync((CoffeeBean?)null);

            // Act
            Func<Task> act = async () => await _service.UpdateBeanAsync(id, mockDto);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                     .WithMessage($"Coffee bean with ID {id} not found.");

            _mockMapper.Verify(m => m.Map(It.IsAny<CreateCoffeeBeanDto>(), It.IsAny<CoffeeBean>()), Times.Never);
            _mockCoffeeBeanRepository.Verify(r => r.UpdateBeanAsync(It.IsAny<CoffeeBean>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteBeanAsync_ExistingId_DeletesAndReturnsTrue()
        {
            // Arrange
            int id = 1;
            var mockBean = new CoffeeBean { Id = id, Name = "FUTURIS" };

            _mockCoffeeBeanRepository.Setup(r => r.GetBeanByIdAsync(id)).ReturnsAsync(mockBean);
            _mockCoffeeBeanRepository.Setup(r => r.DeleteBeanAsync(mockBean)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteBeanAsync(id);

            // Assert
            result.Should().BeTrue();

            _mockCoffeeBeanRepository.Verify(r => r.GetBeanByIdAsync(id), Times.Once);
            _mockCoffeeBeanRepository.Verify(r => r.DeleteBeanAsync(mockBean), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteBeanAsync_NonExistingId_ThrowsNotFoundException()
        {
            // Arrange
            int id = 999;

            _mockCoffeeBeanRepository.Setup(r => r.GetBeanByIdAsync(id)).ReturnsAsync((CoffeeBean?)null);

            // Act
            Func<Task> act = async () => await _service.DeleteBeanAsync(id);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                     .WithMessage($"Coffee bean with ID {id} not found.");

            _mockCoffeeBeanRepository.Verify(r => r.DeleteBeanAsync(It.IsAny<CoffeeBean>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task SearchBeansAsync_QueryProvided_ReturnsMappedResults()
        {
            // Arrange
            string query = "FUTURIS";

            var mockBeans = new List<CoffeeBean>
            {
                new() { Id = 1, Name = "FUTURIS" },
                new() { Id = 2, Name = "ZANITY" }
            };

            var mockMappedDtos = new List<CoffeeBeanDto>
            {
                new() { Id = 1, Name = "FUTURIS" },
                new() { Id = 2, Name = "ZANITY" }
            };

            _mockCoffeeBeanRepository.Setup(r => r.SearchBeansAsync(query))
                               .ReturnsAsync(mockBeans);

            _mockMapper.Setup(m => m.Map<IEnumerable<CoffeeBeanDto>>(mockBeans))
                       .Returns(mockMappedDtos);

            // Act
            var result = await _service.SearchBeansAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(mockMappedDtos);

            _mockCoffeeBeanRepository.Verify(r => r.SearchBeansAsync(query), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<CoffeeBeanDto>>(mockBeans), Times.Once);
        }

        [Fact]
        public async Task GetBeanOfTheDayAsync_ExistingBean_ReturnsMappedBean()
        {
            // Arrange
            var today = DateTime.UtcNow.Date;

            var mockBean = new CoffeeBean { Id = 1, Name = "FUTURIS" };
            var mockBeanOfTheDay = new BeanOfTheDay { CoffeeBeanId = mockBean.Id, Date = today, CoffeeBean = mockBean };
            var mockDto = new CoffeeBeanDto { Id = 1, Name = "FUTURIS" };

            _mockBeanOfTheDayRepository.Setup(r => r.GetBeanByDateAsync(today)).ReturnsAsync(mockBeanOfTheDay);
            _mockMapper.Setup(m => m.Map<CoffeeBeanDto>(mockBean)).Returns(mockDto);

            // Act
            var result = await _service.GetBeanOfTheDayAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(mockDto);

            _mockBeanOfTheDayRepository.Verify(r => r.GetBeanByDateAsync(today), Times.Once);
            _mockMapper.Verify(m => m.Map<CoffeeBeanDto>(mockBean), Times.Once);
            _mockCoffeeBeanRepository.Verify(r => r.GetAllBeansAsync(), Times.Never); // No reselection
        }

        [Fact]
        public async Task GetBeanOfTheDayAsync_NoExisting_SelectsAndSavesNewBean()
        {
            // Arrange
            var today = DateTime.UtcNow.Date;

            var previous = new BeanOfTheDay
            {
                CoffeeBeanId = 1,
                Date = today.AddDays(-1),
                CoffeeBean = new CoffeeBean { Id = 1, Name = "FUTURIS" }
            };

            var allBeans = new List<CoffeeBean>
            {
                new() { Id = 2, Name = "ZANITY" },
                previous.CoffeeBean!
            };

            var selectedBean = allBeans.First(b => b.Id == 2);
            var mockDto = new CoffeeBeanDto { Id = 2, Name = "ZANITY" };

            _mockBeanOfTheDayRepository.Setup(r => r.GetBeanByDateAsync(today)).ReturnsAsync((BeanOfTheDay?)null);
            _mockCoffeeBeanRepository.Setup(r => r.GetAllBeansAsync()).ReturnsAsync(allBeans);
            _mockBeanOfTheDayRepository.Setup(r => r.GetPreviousDayBeanAsync()).ReturnsAsync(previous);
            _mockBeanSelectionStrategy.Setup(s => s.SelectBeanAsync(allBeans, previous.CoffeeBean)).ReturnsAsync(selectedBean);
            _mockMapper.Setup(m => m.Map<CoffeeBeanDto>(selectedBean)).Returns(mockDto);

            // Act
            var result = await _service.GetBeanOfTheDayAsync();

            // Assert
            result.Should().BeEquivalentTo(mockDto);

            _mockBeanOfTheDayRepository.Verify(r => r.AddBeanOfTheDayAsync(It.Is<BeanOfTheDay>(b => b.CoffeeBeanId == selectedBean.Id && b.Date == today)), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }


    }
}
