using Microsoft.EntityFrameworkCore;
using Logistics.Infrastructure.Persistence;
using Logistics.Infrastructure.Persistence.Seed;
using Microsoft.Extensions.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<LogisticsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();


// DATABASE SEEDING
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<LogisticsDbContext>();

    await DatabaseSeeder.SeedAsync(
        dbContext,
        app.Lifetime.ApplicationStopping);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
