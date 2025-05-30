using DirectoryProject.DirectoryService.Domain;
using Microsoft.EntityFrameworkCore;

namespace DirectoryProject.DirectoryService.Infrastructure.Database;

public class ApplicationDBContext : DbContext
{
    public const string DATABASE_CONFIGURATION = "PostgresDB";

    private readonly string _connectionString;

    public ApplicationDBContext(
        string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<DepartmentLocation> DepartmentLocations { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Position> Positions { get; set; }

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
        modelBuilder.HasDefaultSchema("diretory_service");

        modelBuilder.HasPostgresExtension("ltree");  // used in entities: Department

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDBContext).Assembly,
            type => type.FullName?.Contains("Database.Configurations") ?? false);

        base.OnModelCreating(modelBuilder);
    }
}
