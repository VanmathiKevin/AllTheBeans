
using AllTheBeans.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AllTheBeans.Infrastructure.Data
{
    public class DbSeeder
    {
        private readonly BeansDbContext _context;
        private readonly ILogger<DbSeeder> _logger;

        public DbSeeder(BeansDbContext context, ILogger<DbSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task SeedAsync()
        {
            try
            {
                if (await _context.CoffeeBeans.AnyAsync())
                {
                    _logger.LogInformation("Coffee beans already exist in the database. Skipping seeding.");
                    return;
                }

                var path = Path.Combine(AppContext.BaseDirectory, "SeedData", "AllTheBeans.json");

                if (!File.Exists(path))
                {
                    _logger.LogError("Seed file not found at path: {Path}", path);
                    throw new FileNotFoundException($"Seed file not found: {path}");
                }
                    

                var json = await File.ReadAllTextAsync(path);
                var items = JsonConvert.DeserializeObject<List<SeedCoffeeBean>>(json);

                if (items == null || !items.Any())
                {
                    _logger.LogWarning("No seed data found in file: {Path}", path);
                    return;
                }

                foreach (var item in items)
                {
                    if (!decimal.TryParse(item.Cost.Replace("£", ""), out var price))
                    {
                        _logger.LogWarning("Skipping bean '{Name}' due to invalid cost format: '{Cost}'", item.Name, item.Cost);
                        continue;
                    }

                    var bean = new CoffeeBean
                    {
                        Name = item.Name,
                        Colour = item.Colour,
                        Country = item.Country,
                        Description = item.Description,
                        ImageUrl = item.Image,
                        Price = price,
                        IsAvailable = true
                    };

                    _context.CoffeeBeans.Add(bean);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Database seeding completed successfully with {Count} beans.", items.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private class SeedCoffeeBean
        {
            public string Name { get; set; } = default!;
            public string Colour { get; set; } = default!;
            public string Country { get; set; } = default!;
            public string Description { get; set; } = default!;
            public string Image { get; set; } = default!;
            public string Cost { get; set; } = default!;
        }
    }
}
