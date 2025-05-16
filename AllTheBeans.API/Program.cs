using AllTheBeans.Application.Interfaces;
using AllTheBeans.Application.Mapping;
using AllTheBeans.Application.Services;
using AllTheBeans.Infrastructure.Data;
using AllTheBeans.Infrastructure.Repositories;
using AllTheBeans.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// EF Core
builder.Services.AddDbContext<BeansDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

//AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

//Application Services
builder.Services.AddScoped<ICoffeeBeanRepository, CoffeeBeanRepository>();
builder.Services.AddScoped<ICoffeeBeanService, CoffeeBeanService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Load Seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BeansDbContext>();
    await DbSeeder.SeedAsync(db);
}

app.Run();
