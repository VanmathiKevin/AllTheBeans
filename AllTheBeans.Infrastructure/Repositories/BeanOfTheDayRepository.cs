
using AllTheBeans.Application.Interfaces;
using AllTheBeans.Domain.Models;
using AllTheBeans.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AllTheBeans.Infrastructure.Repositories
{
    public class BeanOfTheDayRepository : IBeanOfTheDayRepository
    {
        private readonly BeansDbContext _context;
        public BeanOfTheDayRepository(BeansDbContext context) => _context = context;
        public async Task<BeanOfTheDay?> GetByDateAsync(DateTime date)
        {
            return await _context.BeanOfTheDays
                .Include(b => b.CoffeeBean)
                .FirstOrDefaultAsync(b => b.Date.Date == date.Date);
        }

        public async Task<BeanOfTheDay?> GetPreviousDayAsync()
        {
            var yesterday = DateTime.UtcNow.Date.AddDays(-1);
            return await GetByDateAsync(yesterday);
        }

        public async Task AddAsync(BeanOfTheDay record) =>
            await _context.BeanOfTheDays.AddAsync(record);
    }
}
