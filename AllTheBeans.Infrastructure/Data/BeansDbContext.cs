
using AllTheBeans.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AllTheBeans.Infrastructure.Data
{
    public class BeansDbContext : DbContext
    {
        public BeansDbContext(DbContextOptions<BeansDbContext> options) : base(options) { }

        public DbSet<CoffeeBean> CoffeeBeans { get; set; }
        public DbSet<BeanOfTheDay> BeanOfTheDays { get; set; }
    }
}
