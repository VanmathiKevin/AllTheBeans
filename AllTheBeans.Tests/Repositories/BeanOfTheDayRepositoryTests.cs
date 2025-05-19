
using AllTheBeans.Domain.Models;
using AllTheBeans.Infrastructure.Data;
using AllTheBeans.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AllTheBeans.Tests.Repositories
{
    public class BeanOfTheDayRepositoryTests
    {
        private BeansDbContext CreateDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<BeansDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new BeansDbContext(options);
        }

        private BeanOfTheDayRepository CreateRepository(BeansDbContext context)
        {
            var logger = new LoggerFactory().CreateLogger<BeanOfTheDayRepository>();
            return new BeanOfTheDayRepository(context, logger);
        }

        [Fact]
        public async Task GetBeanByDateAsync_WithValidDate_ReturnsRecord()
        {
            var db = CreateDbContext("GetBeanByDateTest");

            var bean = new CoffeeBean { Id = 1, Name = "FUTURIS", Colour = "Dark", Country = "Colombia", Description = "Bold", ImageUrl = "url", Price = 9.9m };
            var record = new BeanOfTheDay { CoffeeBeanId = 1, Date = DateTime.UtcNow.Date, CoffeeBean = bean };

            db.CoffeeBeans.Add(bean);
            db.BeanOfTheDays.Add(record);
            await db.SaveChangesAsync();

            var repo = CreateRepository(db);
            var result = await repo.GetBeanByDateAsync(DateTime.UtcNow.Date);

            result.Should().NotBeNull();
            result!.CoffeeBeanId.Should().Be(1);
            result.CoffeeBean.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPreviousDayBeanAsync_ReturnsYesterdayRecord()
        {
            var db = CreateDbContext("GetPreviousDayBeanTest");
            var yesterday = DateTime.UtcNow.Date.AddDays(-1);

            var bean = new CoffeeBean { Id = 1, Name = "ZANITY", Colour = "Light", Country = "Ethiopia", Description = "Floral", ImageUrl = "url", Price = 10.0m };
            var record = new BeanOfTheDay { CoffeeBeanId = 1, Date = yesterday, CoffeeBean = bean };

            db.CoffeeBeans.Add(bean);
            db.BeanOfTheDays.Add(record);
            await db.SaveChangesAsync();

            var repo = CreateRepository(db);
            var result = await repo.GetPreviousDayBeanAsync();

            result.Should().NotBeNull();
            result!.Date.Should().Be(yesterday);
        }

        [Fact]
        public async Task AddBeanOfTheDayAsync_AddsRecord()
        {
            var db = CreateDbContext("AddBeanOfTheDayTest");

            var bean = new CoffeeBean { Id = 1, Name = "FUTURIS", Colour = "Brown", Country = "India", Description = "Nutty", ImageUrl = "url", Price = 8.5m };
            db.CoffeeBeans.Add(bean);
            await db.SaveChangesAsync();

            var record = new BeanOfTheDay { CoffeeBeanId = 1, Date = DateTime.UtcNow.Date };

            var repo = CreateRepository(db);
            await repo.AddBeanOfTheDayAsync(record);
            await db.SaveChangesAsync();

            db.BeanOfTheDays.Should().ContainSingle(b => b.CoffeeBeanId == 1 && b.Date == DateTime.UtcNow.Date);
        }

    }
}
