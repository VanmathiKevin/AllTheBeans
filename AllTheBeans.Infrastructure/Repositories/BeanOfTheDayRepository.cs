
using AllTheBeans.Application.Interfaces;
using AllTheBeans.Domain.Models;
using AllTheBeans.Infrastructure.Data;
using AllTheBeans.Infrastructure.Exceptions;
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

            try
            {
                return await _context.BeanOfTheDays
                .Include(b => b.CoffeeBean)
                .FirstOrDefaultAsync(b => b.Date.Date == date.Date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching BeanOfTheDay for date {Date}", date);
                throw new DataAccessException($"Failed to fetch BeanOfTheDay for {date:yyyy-MM-dd}.", ex);
            }
        }

        public async Task<BeanOfTheDay?> GetPreviousDayBeanAsync()
        {
            var yesterday = DateTime.UtcNow.Date.AddDays(-1);
            _logger.LogInformation("Fetching previous day's BeanOfTheDay (Date: {Date})", yesterday);
            try
            {
                return await GetBeanByDateAsync(yesterday);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching previous day's BeanOfTheDay");
                throw;
            }
        }

        public async Task AddBeanOfTheDayAsync(BeanOfTheDay record)
        {
            _logger.LogInformation("Adding BeanOfTheDay for date {Date} with CoffeeBean ID {CoffeeBeanId}", record.Date, record.CoffeeBeanId);
            try
            {
                await _context.BeanOfTheDays.AddAsync(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding BeanOfTheDay for date {Date}", record.Date);
                throw new DataAccessException("Failed to add BeanOfTheDay record.", ex);
            }
        }
    }
}
