
using AllTheBeans.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AllTheBeans.Infrastructure.Data
{
    public class BeansDbContext : DbContext
    {
        public BeansDbContext(DbContextOptions<BeansDbContext> options) : base(options) { }

        public DbSet<CoffeeBean> CoffeeBeans { get; set; }
        public DbSet<BeanOfTheDay> BeanOfTheDays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure decimal precision for Price
            modelBuilder.Entity<CoffeeBean>(entity =>
            {
                entity.Property(e => e.Price)
                      .HasPrecision(18, 4); // precision 18, scale 4
            });

        }
    }
}
