
using AllTheBeans.Application.Interfaces;
using AllTheBeans.Domain.Models;
using AllTheBeans.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AllTheBeans.Infrastructure.Repositories
{
    public class CoffeeBeanRepository : ICoffeeBeanRepository
    {
        private readonly BeansDbContext _context;
        private readonly ILogger<CoffeeBeanRepository> _logger;
        public CoffeeBeanRepository(BeansDbContext context, ILogger<CoffeeBeanRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CoffeeBean>> GetAllBeansAsync()
        {
            _logger.LogInformation("Fetching all coffee beans from database");
            return await _context.CoffeeBeans.Where(b => b.IsAvailable).ToListAsync();
        }
            
        public async Task<CoffeeBean?> GetBeanByIdAsync(int id)
        {
            _logger.LogInformation("Fetching coffee bean with ID {Id} from database", id);
            var bean = await _context.CoffeeBeans.FindAsync(id);
            if (bean == null)
                _logger.LogWarning("Coffee bean with ID {Id} not found", id);
            return bean;
        }

        public async Task AddBeanAsync(CoffeeBean bean)
        {
            _logger.LogInformation("Adding new coffee bean: {Name}", bean.Name);
            await _context.CoffeeBeans.AddAsync(bean);
        }
            
        public async Task UpdateBeanAsync(CoffeeBean bean)
        {
            _logger.LogInformation("Updating coffee bean with ID {Id}", bean.Id);
            _context.CoffeeBeans.Update(bean);
            await Task.CompletedTask;
        }
        public async Task DeleteBeanAsync(CoffeeBean bean)
        {
            _logger.LogInformation("Deleting coffee bean with ID {Id}", bean.Id);
            _context.CoffeeBeans.Remove(bean);
            await Task.CompletedTask;
        }
        public async Task<IEnumerable<CoffeeBean>> SearchBeansAsync(string keyword)
        {
            _logger.LogInformation("Searching coffee beans with query '{Query}'", keyword);
            return await _context.CoffeeBeans
                .Where(b => b.Name.Contains(keyword) ||
                            b.Country.Contains(keyword) ||
                            b.Colour.Contains(keyword))
                .ToListAsync();
        }
    }
}
