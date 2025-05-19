
using AllTheBeans.Application.Interfaces;
using AllTheBeans.Domain.Models;
using AllTheBeans.Infrastructure.Data;
using AllTheBeans.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            try
            {
                return await _context.CoffeeBeans
                    .Where(b => b.IsAvailable)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all coffee beans.");
                throw new DataAccessException("Failed to fetch all coffee beans.", ex);
            }
        }
            
        public async Task<CoffeeBean?> GetBeanByIdAsync(int id)
        {
            _logger.LogInformation("Fetching coffee bean with ID {Id} from database", id);

            try
            {
                var bean = await _context.CoffeeBeans.FindAsync(id);
                if (bean == null)
                    _logger.LogWarning("Coffee bean with ID {Id} not found", id);
                return bean;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching coffee bean with ID {Id}", id);
                throw new DataAccessException($"Failed to retrieve coffee bean with ID {id}.", ex);
            }
        }

        public async Task AddBeanAsync(CoffeeBean bean)
        {
            _logger.LogInformation("Adding new coffee bean: {Name}", bean.Name);
            try
            {
                await _context.CoffeeBeans.AddAsync(bean);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding coffee bean: {Name}", bean.Name);
                throw new DataAccessException("Failed to add new coffee bean.", ex);
            }
        }
            
        public async Task UpdateBeanAsync(CoffeeBean bean)
        {
            _logger.LogInformation("Updating coffee bean with ID {Id}", bean.Id);

            try
            {
                _context.CoffeeBeans.Update(bean);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating coffee bean with ID {Id}", bean.Id);
                throw new DataAccessException($"Failed to update coffee bean with ID {bean.Id}.", ex);
            }
        }
        public async Task DeleteBeanAsync(CoffeeBean bean)
        {
            _logger.LogInformation("Deleting coffee bean with ID {Id}", bean.Id);
            try
            {
                _context.CoffeeBeans.Remove(bean);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting coffee bean with ID {Id}", bean.Id);
                throw new DataAccessException($"Failed to delete coffee bean with ID {bean.Id}.", ex);
            }
        }
        public async Task<IEnumerable<CoffeeBean>> SearchBeansAsync(string keyword)
        {
            _logger.LogInformation("Searching coffee beans with keyword '{Keyword}'", keyword);

            try 
            {
                return await _context.CoffeeBeans
                .Where(b => b.Name.ToLower().Contains(keyword) ||
                            b.Country.ToLower().Contains(keyword) ||
                            b.Colour.ToLower().Contains(keyword))
                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during search with keyword: {Keyword}", keyword);
                throw new DataAccessException("Search operation failed.", ex);
            }
        }
    }
}
