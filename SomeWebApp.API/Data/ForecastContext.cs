using Microsoft.EntityFrameworkCore;

namespace SomeWebApp.API.Data;

public class ForecastContext : DbContext
{
    internal DbSet<Forecast> Forecasts { get; set; }

    public ForecastContext(DbContextOptions<ForecastContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Forecast>().ToTable("Forecast");
    }
}