
using AllTheBeans.Application.Interfaces;
using AllTheBeans.Domain.Models;
using System;

namespace AllTheBeans.Application.Services
{
    public class BeanSelectorService : IBeanSelectorService
    {
        private readonly Random _random = new();
        public Task<CoffeeBean> SelectBeanAsync(IEnumerable<CoffeeBean> availableBeans, CoffeeBean? previousBean)
        {
            if (availableBeans == null || !availableBeans.Any())
                throw new InvalidOperationException("No coffee beans available to select from.");

            var qualifiedBeans = availableBeans
                .Where(b => previousBean == null || b.Id != previousBean.Id)
                .ToList();

            if (!qualifiedBeans.Any())
                throw new InvalidOperationException("No alternative coffee beans available to avoid repetition.");

            var selected = qualifiedBeans[_random.Next(qualifiedBeans.Count)];
            return Task.FromResult(selected);
        }
    }
}
