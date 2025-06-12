using DirectoryProject.DirectoryService.Domain;
using Microsoft.EntityFrameworkCore;

namespace DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite;

public class ApplicationWriteDBContext : DbContext
{
    public const string DATABASE_CONFIGURATION = "PostgresDB";

    private readonly string _connectionString;

    public ApplicationWriteDBContext(
        string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<DepartmentLocation> DepartmentLocations { get; set; }
    public DbSet<DepartmentPosition> DepartmentPositions { get; set; }
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
            typeof(ApplicationWriteDBContext).Assembly,
            type => type.GetInterfaces().Any(i => i.Name == typeof(IEntityTypeConfiguration<>).Name));

        base.OnModelCreating(modelBuilder);
    }
}
