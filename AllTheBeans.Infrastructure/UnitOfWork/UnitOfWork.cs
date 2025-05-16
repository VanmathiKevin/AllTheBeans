using AllTheBeans.Application.Interfaces;
using AllTheBeans.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace AllTheBeans.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BeansDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        public UnitOfWork(BeansDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("Saving changes to the database");
            await _context.SaveChangesAsync();
        } 
    }
}
