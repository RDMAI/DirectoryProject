using Microsoft.EntityFrameworkCore;
using OrderService.WebAPI.Domain;

namespace OrderService.WebAPI.Database;

public class ApplicationDBContext : DbContext
{
    public const string DATABASE_CONFIGURATION = "PostgresDB";

    private readonly string _connectionString;

    public ApplicationDBContext(
        string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public async Task ApplyMigrationsAsync()
    {
        await Database.MigrateAsync();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.LogTo(Console.WriteLine)
            .EnableSensitiveDataLogging();
        optionsBuilder.UseSnakeCaseNamingConvention();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("order_service");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDBContext).Assembly,
            type => type.GetInterfaces().Any(i => i.Name == typeof(IEntityTypeConfiguration<>).Name));

        base.OnModelCreating(modelBuilder);
    }
}
