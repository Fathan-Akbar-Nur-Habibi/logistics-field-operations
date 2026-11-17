using Microsoft.EntityFrameworkCore;
using Logistics.Infrastructure.Persistence;
using Logistics.Infrastructure.Persistence.Seed;
using Microsoft.Extensions.DependencyInjection;
using Logistics.Application.Interfaces.Inventory;
using Logistics.Application.Services.Inventory;
using Logistics.Infrastructure.Persistence.Repositories.Inventory;
using Logistics.Application.Interfaces.Asset;
using Logistics.Infrastructure.Persistence.Repositories.Asset;
using Logistics.Application.Services.Asset;
using Logistics.Application.Interfaces.Maintenance;
using Logistics.Application.Services.Maintenance;
using Logistics.Infrastructure.Persistence.Repositories.Maintenance;
using Logistics.Application.Interfaces.Crew;
using Logistics.Application.Services.Crew;
using Logistics.Infrastructure.Persistence.Repositories.Crew;
using Logistics.Application.Interfaces.Logistics;
using Logistics.Application.Services.Logistics;
using Logistics.Infrastructure.Persistence.Repositories.Logistics;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LogisticsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IAssetService, AssetService>();
// Maintenance
builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
// Crew Management
builder.Services.AddScoped<ICrewRepository, CrewRepository>();
builder.Services.AddScoped<ICrewService, CrewService>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IShiftService, ShiftService>();
// Crew Schedule
builder.Services.AddScoped<ICrewScheduleRepository, CrewScheduleRepository>();
builder.Services.AddScoped<ICrewScheduleService, CrewScheduleService>();
// Field Assignment
builder.Services.AddScoped<IFieldAssignmentRepository, FieldAssignmentRepository>();
builder.Services.AddScoped<IFieldAssignmentService, FieldAssignmentService>();
// Logistics
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
