
using AllTheBeans.Application.Interfaces;
using AllTheBeans.Domain.Models;
using AllTheBeans.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AllTheBeans.Infrastructure.Repositories
{
    public class CoffeeBeanRepository : ICoffeeBeanRepository
    {
        private readonly BeansDbContext _context;
        public CoffeeBeanRepository(BeansDbContext context) => _context = context;

        public async Task<IEnumerable<CoffeeBean>> GetAllAsync() =>
            await _context.CoffeeBeans.Where(b => b.IsAvailable).ToListAsync();

        public async Task<CoffeeBean?> GetByIdAsync(int id) =>
            await _context.CoffeeBeans.FindAsync(id);

        public async Task AddAsync(CoffeeBean bean) =>
            await _context.CoffeeBeans.AddAsync(bean);
        public Task UpdateAsync(CoffeeBean bean)
        {
            _context.CoffeeBeans.Update(bean);
            return Task.CompletedTask;
        }
        public Task DeleteAsync(CoffeeBean bean)
        {
            _context.CoffeeBeans.Remove(bean);
            return Task.CompletedTask;
        }
    }
}
