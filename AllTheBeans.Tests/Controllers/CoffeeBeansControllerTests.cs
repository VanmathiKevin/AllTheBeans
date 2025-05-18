
using AllTheBeans.API.Controllers;
using AllTheBeans.Application.DTOs;
using AllTheBeans.Application.Exceptions;
using AllTheBeans.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AllTheBeans.Tests.Controllers
{
    public class CoffeeBeansControllerTests
    {
        private readonly Mock<ICoffeeBeanService> _mockService = new();
        private readonly Mock<ICacheService> _mockCache = new();
        private readonly Mock<ILogger<CoffeeBeansController>> _mockLogger = new();

        private readonly CoffeeBeansController _controller;

        public CoffeeBeansControllerTests()
        {
            _controller = new CoffeeBeansController(
                _mockService.Object,
                _mockLogger.Object,
                _mockCache.Object
            );
        }

        [Fact]
        public async Task GetAll_WhenCached_ReturnsCachedBeans()
        {
            // Arrange
            var cachedBeans = new List<CoffeeBeanDto>
            {
                new() { Id = 1, Name = "FUTURIS" }
            };

            _mockCache.Setup(c => c.GetAsync<IEnumerable<CoffeeBeanDto>>("AllCoffeeBeans"))
                      .ReturnsAsync(cachedBeans);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(cachedBeans);

            _mockService.Verify(s => s.GetAllBeansAsync(), Times.Never);
            _mockCache.Verify(c => c.GetAsync<IEnumerable<CoffeeBeanDto>>("AllCoffeeBeans"), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenNotCached_CallsServiceAndSetsCache()
        {
            // Arrange
            var beans = new List<CoffeeBeanDto>
            {
                new() { Id = 1, Name = "FUTURIS" }
            };

            _mockCache.Setup(c => c.GetAsync<IEnumerable<CoffeeBeanDto>>("AllCoffeeBeans"))
                      .ReturnsAsync((IEnumerable<CoffeeBeanDto>?)null);

            _mockService.Setup(s => s.GetAllBeansAsync()).ReturnsAsync(beans);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(beans);

            _mockService.Verify(s => s.GetAllBeansAsync(), Times.Once);
            _mockCache.Verify(c => c.SetAsync<IEnumerable<CoffeeBeanDto>>("AllCoffeeBeans", beans, TimeSpan.FromMinutes(10)), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenCached_ReturnsFromCache()
        {
            // Arrange
            int id = 1;
            var cachedDto = new CoffeeBeanDto { Id = id, Name = "FUTURIS" };
            _mockCache.Setup(c => c.GetAsync<CoffeeBeanDto>($"CoffeeBean:{id}"))
                      .ReturnsAsync(cachedDto);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(cachedDto);

            _mockService.Verify(s => s.GetBeanByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetById_WhenNotCached_FetchesFromServiceAndCaches()
        {
            // Arrange
            int id = 2;
            var serviceDto = new CoffeeBeanDto { Id = id, Name = "ZANITY" };

            _mockCache.Setup(c => c.GetAsync<CoffeeBeanDto>($"CoffeeBean:{id}"))
                      .ReturnsAsync((CoffeeBeanDto?)null);

            _mockService.Setup(s => s.GetBeanByIdAsync(id)).ReturnsAsync(serviceDto);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(serviceDto);

            _mockService.Verify(s => s.GetBeanByIdAsync(id), Times.Once);
            _mockCache.Verify(c => c.SetAsync($"CoffeeBean:{id}", serviceDto, TimeSpan.FromMinutes(10)), Times.Once);
        }

        [Fact]
        public async Task Create_ValidDto_ReturnsCreatedAtActionWithDto()
        {
            // Arrange
            var dto = new CreateCoffeeBeanDto { Name = "FUTURIS", Price = 12.5m };
            var createdDto = new CoffeeBeanDto { Id = 1, Name = "FUTURIS", Price = 12.5m };

            _mockService.Setup(s => s.CreateBeanAsync(dto)).ReturnsAsync(createdDto);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var created = result as CreatedAtActionResult;
            created.Should().NotBeNull();
            created!.ActionName.Should().Be(nameof(_controller.GetById));
            created.RouteValues!["id"].Should().Be(createdDto.Id);
            created.Value.Should().BeEquivalentTo(createdDto);

            _mockService.Verify(s => s.CreateBeanAsync(dto), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync("AllCoffeeBeans"), Times.Once);
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            var dto = new CreateCoffeeBeanDto(); // Missing required fields

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.StatusCode.Should().Be(400);

            _mockService.Verify(s => s.CreateBeanAsync(It.IsAny<CreateCoffeeBeanDto>()), Times.Never);
            _mockCache.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Update_ValidDto_ReturnsNoContent()
        {
            // Arrange
            int id = 1;
            var dto = new CreateCoffeeBeanDto { Name = "FUTURIS", Price = 9.9m };

            _mockService.Setup(s => s.UpdateBeanAsync(id, dto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            result.Should().BeOfType<NoContentResult>();

            _mockService.Verify(s => s.UpdateBeanAsync(id, dto), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync("AllCoffeeBeans"), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync($"CoffeeBean:{id}"), Times.Once);
        }

        [Fact]
        public async Task Update_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            int id = 1;
            var dto = new CreateCoffeeBeanDto(); // Missing required fields
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

            _mockService.Verify(s => s.UpdateBeanAsync(It.IsAny<int>(), It.IsAny<CreateCoffeeBeanDto>()), Times.Never);
            _mockCache.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Delete_ExistingId_ReturnsNoContent()
        {
            // Arrange
            int id = 1;

            _mockService.Setup(s => s.DeleteBeanAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();

            _mockService.Verify(s => s.DeleteBeanAsync(id), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync("AllCoffeeBeans"), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync($"CoffeeBean:{id}"), Times.Once);
        }

        [Fact]
        public async Task Delete_NonExistingId_ThrowsNotFoundException()
        {
            // Arrange
            int id = 999;

            _mockService.Setup(s => s.DeleteBeanAsync(id))
                        .ThrowsAsync(new NotFoundException($"Coffee bean with ID {id} not found."));

            // Act
            Func<Task> act = async () => await _controller.Delete(id);

            // Assert
            await act.Should()
                     .ThrowAsync<NotFoundException>()
                     .WithMessage($"Coffee bean with ID {id} not found.");

            _mockCache.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Search_WhenCached_ReturnsFromCache()
        {
            // Arrange
            string query = "futuris";
            string cacheKey = $"Search:{query}";

            var cachedResults = new List<CoffeeBeanDto>
            {
                new() { Id = 1, Name = "FUTURIS" }
            };

            _mockCache.Setup(c => c.GetAsync<IEnumerable<CoffeeBeanDto>>(cacheKey))
                      .ReturnsAsync(cachedResults);

            // Act
            var result = await _controller.Search(query);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(cachedResults);

            _mockService.Verify(s => s.SearchBeansAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Search_WhenNotCached_CallsServiceAndSetsCache()
        {
            // Arrange
            string query = "zanity";
            string cacheKey = $"Search:{query}";

            var searchResults = new List<CoffeeBeanDto>
            {
                new() { Id = 2, Name = "ZANITY" }
            };

            _mockCache.Setup(c => c.GetAsync<IEnumerable<CoffeeBeanDto>>(cacheKey))
                      .ReturnsAsync((IEnumerable<CoffeeBeanDto>?)null);

            _mockService.Setup(s => s.SearchBeansAsync(query)).ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(query);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(searchResults);

            _mockService.Verify(s => s.SearchBeansAsync(query), Times.Once);
            _mockCache.Verify(c => c.SetAsync<IEnumerable<CoffeeBeanDto>>(
                "Search:zanity",
                It.IsAny<IEnumerable<CoffeeBeanDto>>(),
                TimeSpan.FromMinutes(5)),
            Times.Once);
        }

        [Fact]
        public async Task GetBeanOfTheDay_WhenCached_ReturnsFromCache()
        {
            // Arrange
            var todayKey = $"BeanOfTheDay:{DateTime.UtcNow:yyyy-MM-dd}";
            var cachedBean = new CoffeeBeanDto { Id = 1, Name = "FUTURIS" };

            _mockCache.Setup(c => c.GetAsync<CoffeeBeanDto>(todayKey))
                      .ReturnsAsync(cachedBean);

            // Act
            var result = await _controller.GetBeanOfTheDay();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(cachedBean);

            _mockService.Verify(s => s.GetBeanOfTheDayAsync(), Times.Never);
            _mockCache.Verify(c => c.GetAsync<CoffeeBeanDto>(todayKey), Times.Once);
        }

        [Fact]
        public async Task GetBeanOfTheDay_WhenNotCached_CallsServiceAndSetsCache()
        {
            // Arrange
            var todayKey = $"BeanOfTheDay:{DateTime.UtcNow:yyyy-MM-dd}";
            var bean = new CoffeeBeanDto { Id = 2, Name = "ZANITY" };

            _mockCache.Setup(c => c.GetAsync<CoffeeBeanDto>(todayKey))
                      .ReturnsAsync((CoffeeBeanDto?)null);

            _mockService.Setup(s => s.GetBeanOfTheDayAsync()).ReturnsAsync(bean);

            // Act
            var result = await _controller.GetBeanOfTheDay();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(bean);

            _mockService.Verify(s => s.GetBeanOfTheDayAsync(), Times.Once);
            _mockCache.Verify(c => c.SetAsync(todayKey, bean, TimeSpan.FromDays(1)), Times.Once);
        }


    }
}
