
using AllTheBeans.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AllTheBeans.Infrastructure.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(BeansDbContext context)
        {
            if (await context.CoffeeBeans.AnyAsync())
                return;

            var path = Path.Combine(AppContext.BaseDirectory, "SeedData", "AllTheBeans.json");

            if (!File.Exists(path))
                throw new FileNotFoundException($"Seed file not found: {path}");

            var json = await File.ReadAllTextAsync(path);
            var items = JsonConvert.DeserializeObject<List<SeedCoffeeBean>>(json);

            if (items is null) return;

            foreach (var item in items)
            {
                if (!decimal.TryParse(item.Cost.Replace("£", ""), out var price)) continue;

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

                context.CoffeeBeans.Add(bean);
            }

            await context.SaveChangesAsync();
        }

        private class SeedCoffeeBean
        {
            public string Name { get; set; }
            public string Colour { get; set; }
            public string Country { get; set; }
            public string Description { get; set; }
            public string Image { get; set; }
            public string Cost { get; set; }
        }
    }
}
