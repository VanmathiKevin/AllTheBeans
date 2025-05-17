
using AllTheBeans.Application.Exceptions;
using AllTheBeans.Application.Interfaces;
using AllTheBeans.Domain.Models;
using Microsoft.Extensions.Logging;
using System;

namespace AllTheBeans.Application.Services
{
    public class BeanSelectorService : IBeanSelectorService
    {
        private readonly Random _random = new();
        private readonly ILogger<BeanSelectorService> _logger;

        public BeanSelectorService(ILogger<BeanSelectorService> logger)
        {
            _logger = logger;
        }
        public Task<CoffeeBean> SelectBeanAsync(IEnumerable<CoffeeBean> availableBeans, CoffeeBean? previousBean)
        {
            try
            {
                if (availableBeans == null || !availableBeans.Any())
                {
                    _logger.LogWarning("Bean selection failed: No coffee beans available to select from.");
                    throw new InvalidBeanSelectionException("No coffee beans available to select from.");
                }

                _logger.LogInformation("Selecting a bean of the day from {Count} available beans", availableBeans.Count());

                var qualifiedBeans = availableBeans
                    .Where(b => previousBean == null || b.Id != previousBean.Id)
                    .ToList();

                if (!qualifiedBeans.Any())
                {
                    _logger.LogWarning("No alternative coffee beans available to avoid repetition.");
                    throw new InvalidBeanSelectionException("No alternative coffee beans available to avoid repetition.");
                }

                var selectedBean = qualifiedBeans[_random.Next(qualifiedBeans.Count)];
                _logger.LogInformation("Selected bean: {BeanName} (ID: {BeanId}) as Bean of the Day", selectedBean.Name, selectedBean.Id);

                return Task.FromResult(selectedBean);
            }
            catch (InvalidBeanSelectionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during bean selection.");
                throw new InvalidBeanSelectionException("An unexpected error occurred while selecting the bean of the day.", ex);
            }
        }
    }
}
