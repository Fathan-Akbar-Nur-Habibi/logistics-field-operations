
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence;

public class LogisticsDbContext : DbContext
{
    public LogisticsDbContext(
        DbContextOptions<LogisticsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
