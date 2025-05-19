
using AllTheBeans.Domain.Models;
using AllTheBeans.Infrastructure.Data;
using AllTheBeans.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AllTheBeans.Tests.Repositories
{
    public class CoffeeBeanRepositoryTests
    {
        private BeansDbContext CreateDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<BeansDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new BeansDbContext(options);
        }

        private CoffeeBeanRepository CreateRepository(BeansDbContext context)
        {
            var logger = new LoggerFactory().CreateLogger<CoffeeBeanRepository>();
            return new CoffeeBeanRepository(context, logger);
        }

        [Fact]
        public async Task GetAllBeansAsync_ReturnsOnlyAvailableBeans()
        {
            // Arrange
            var db = CreateDbContext("GetAllBeansTest");
            db.CoffeeBeans.AddRange(
            new CoffeeBean
            {
                Id = 1,
                Name = "FUTURIS",
                IsAvailable = true,
                Colour = "Dark",
                Country = "Colombia",
                Description = "Rich and bold",
                ImageUrl = "https://example.com/futuris.jpg",
                Price = 10.5m
            },
            new CoffeeBean
            {
                Id = 2,
                Name = "ZANITY",
                IsAvailable = false,
                Colour = "Light",
                Country = "Kenya",
                Description = "Floral and bright",
                ImageUrl = "https://example.com/zanity.jpg",
                Price = 9.0m
            }
        );
            await db.SaveChangesAsync();

            var repo = CreateRepository(db);

            // Act
            var result = await repo.GetAllBeansAsync();

            // Assert
            result.Should().HaveCount(1);
            result.Should().ContainSingle(b => b.Name == "FUTURIS");
        }

        [Fact]
        public async Task GetBeanByIdAsync_ExistingId_ReturnsBean()
        {
            var db = CreateDbContext("GetBeanByIdTest");
            var bean = new CoffeeBean
            {
                Id = 1,
                Name = "FUTURIS",
                IsAvailable = true,
                Colour = "Dark",
                Country = "Colombia",
                Description = "Rich and bold",
                ImageUrl = "https://example.com/futuris.jpg",
                Price = 10.5m
            };
            db.CoffeeBeans.Add(bean);
            await db.SaveChangesAsync();

            var repo = CreateRepository(db);

            var result = await repo.GetBeanByIdAsync(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("FUTURIS");
        }

        [Fact]
        public async Task AddBeanAsync_AddsNewBean()
        {
            var db = CreateDbContext("AddBeanTest");
            var repo = CreateRepository(db);

            var bean = new CoffeeBean
            {
                Id = 1,
                Name = "FUTURIS",
                IsAvailable = true,
                Colour = "Dark",
                Country = "Colombia",
                Description = "Rich and bold",
                ImageUrl = "https://example.com/futuris.jpg",
                Price = 10.5m
            };

            await repo.AddBeanAsync(bean);
            await db.SaveChangesAsync();

            db.CoffeeBeans.Should().ContainSingle(b => b.Name == "FUTURIS");
        }

        [Fact]
        public async Task UpdateBeanAsync_UpdatesEntity()
        {
            var db = CreateDbContext("UpdateBeanTest");
            var bean = new CoffeeBean
            {
                Id = 1,
                Name = "FUTURIS",
                IsAvailable = true,
                Colour = "Dark",
                Country = "Colombia",
                Description = "Rich and bold",
                ImageUrl = "https://example.com/futuris.jpg",
                Price = 10.5m
            };
            db.CoffeeBeans.Add(bean);
            await db.SaveChangesAsync();

            bean.Name = "ZANITY";
            var repo = CreateRepository(db);
            await repo.UpdateBeanAsync(bean);
            await db.SaveChangesAsync();

            db.CoffeeBeans.First().Name.Should().Be("ZANITY");
        }

        [Fact]
        public async Task DeleteBeanAsync_RemovesEntity()
        {
            var db = CreateDbContext("DeleteBeanTest");
            var bean = new CoffeeBean
            {
                Id = 1,
                Name = "FUTURIS",
                IsAvailable = true,
                Colour = "Dark",
                Country = "Colombia",
                Description = "Rich and bold",
                ImageUrl = "https://example.com/futuris.jpg",
                Price = 10.5m
            };
            db.CoffeeBeans.Add(bean);
            await db.SaveChangesAsync();

            var repo = CreateRepository(db);
            await repo.DeleteBeanAsync(bean);
            await db.SaveChangesAsync();

            db.CoffeeBeans.Should().BeEmpty();
        }

        [Fact]
        public async Task SearchBeansAsync_MatchesByNameCountryOrColour()
        {
            var db = CreateDbContext("SearchBeansTest");
            db.CoffeeBeans.AddRange(
            new CoffeeBean
            {
                Id = 1,
                Name = "FUTURIS",
                IsAvailable = true,
                Colour = "Dark",
                Country = "Colombia",
                Description = "Rich and bold",
                ImageUrl = "https://example.com/futuris.jpg",
                Price = 10.5m
            },
            new CoffeeBean
            {
                Id = 2,
                Name = "ZANITY",
                IsAvailable = false,
                Colour = "Light",
                Country = "Kenya",
                Description = "Floral and bright",
                ImageUrl = "https://example.com/zanity.jpg",
                Price = 9.0m
            }
        );
            await db.SaveChangesAsync();

            var repo = CreateRepository(db);
            var result = await repo.SearchBeansAsync("colomb");

            result.Should().ContainSingle(b => b.Country == "Colombia");
        }


    }
}
