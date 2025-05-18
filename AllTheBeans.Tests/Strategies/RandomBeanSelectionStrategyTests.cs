
using AllTheBeans.Application.Exceptions;
using AllTheBeans.Application.Strategy;
using AllTheBeans.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AllTheBeans.Tests.Strategies
{
    public class RandomBeanSelectionStrategyTests
    {
        private readonly Mock<ILogger<RandomBeanSelectionStrategy>> _mockLogger = new();
        private readonly RandomBeanSelectionStrategy _mockRandomStrategy;

        public RandomBeanSelectionStrategyTests()
        {
            _mockRandomStrategy = new RandomBeanSelectionStrategy(_mockLogger.Object);
        }

        [Fact]
        public async Task SelectBeanAsync_WithValidBeansAndPrevious_ReturnsDifferentBean()
        {
            // Arrange
            var previousBean = new CoffeeBean { Id = 1, Name = "FUTURIS" };
            var beans = new List<CoffeeBean>
            {
                previousBean,
                new() { Id = 2, Name = "ZANITY" },
                new() { Id = 3, Name = "KLUGGER" }
            };

            // Act
            var selected = await _mockRandomStrategy.SelectBeanAsync(beans, previousBean);

            // Assert
            selected.Should().NotBeNull();
            selected.Id.Should().NotBe(previousBean.Id);
        }

        [Fact]
        public async Task SelectBeanAsync_WithEmptyList_ThrowsInvalidBeanSelectionException()
        {
            // Arrange
            var emptyList = new List<CoffeeBean>();

            // Act
            Func<Task> act = async () => await _mockRandomStrategy.SelectBeanAsync(emptyList, null);

            // Assert
            await act.Should().ThrowAsync<InvalidBeanSelectionException>()
                     .WithMessage("No coffee beans available to select from.");
        }

        [Fact]
        public async Task SelectBeanAsync_OnlyPreviousAvailable_ThrowsInvalidBeanSelectionException()
        {
            // Arrange
            var mockBean = new CoffeeBean { Id = 1, Name = "FUTURIS" };
            var mockLlist = new List<CoffeeBean> { mockBean };

            // Act
            Func<Task> act = async () => await _mockRandomStrategy.SelectBeanAsync(mockLlist, mockBean);

            // Assert
            await act.Should().ThrowAsync<InvalidBeanSelectionException>()
                     .WithMessage("No alternative coffee beans available to avoid repetition.");
        }

        [Fact]
        public async Task SelectBeanAsync_NoPrevious_ReturnsAnyBean()
        {
            // Arrange
            var beans = new List<CoffeeBean>
            {
                new() { Id = 1, Name = "FUTURIS" },
                new() { Id = 2, Name = "ZANITY" }
            };

            // Act
            var selected = await _mockRandomStrategy.SelectBeanAsync(beans, null);

            // Assert
            selected.Should().NotBeNull();
            beans.Should().Contain(b => b.Id == selected.Id);
        }


    }
}
