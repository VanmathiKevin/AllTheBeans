
using AllTheBeans.Application.Interfaces;
using AllTheBeans.Domain.Models;
using AllTheBeans.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AllTheBeans.Infrastructure.Repositories
{
    public class BeanOfTheDayRepository : IBeanOfTheDayRepository
    {
        private readonly BeansDbContext _context;
        private readonly ILogger<BeanOfTheDayRepository> _logger;
        public BeanOfTheDayRepository(BeansDbContext context, ILogger<BeanOfTheDayRepository> logger)
        {
            _context = context;
            _logger = logger;
        } 
        public async Task<BeanOfTheDay?> GetBeanByDateAsync(DateTime date)
        {
            _logger.LogInformation("Fetching BeanOfTheDay for date {Date}", date);
            return await _context.BeanOfTheDays
                .Include(b => b.CoffeeBean)
                .FirstOrDefaultAsync(b => b.Date.Date == date.Date);
        }

        public async Task<BeanOfTheDay?> GetPreviousDayBeanAsync()
        {
            _logger.LogInformation("Fetching previous BeanOfTheDay");
            var yesterday = DateTime.UtcNow.Date.AddDays(-1);
            return await GetBeanByDateAsync(yesterday);
        }

        public async Task AddBeanOfTheDayAsync(BeanOfTheDay record) =>
            await _context.BeanOfTheDays.AddAsync(record);
    }
}
