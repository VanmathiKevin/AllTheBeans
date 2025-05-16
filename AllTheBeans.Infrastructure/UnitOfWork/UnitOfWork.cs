using AllTheBeans.Application.Interfaces;
using AllTheBeans.Infrastructure.Data;

namespace AllTheBeans.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BeansDbContext _context;
        public UnitOfWork(BeansDbContext context) => _context = context;

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
