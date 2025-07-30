using InventoryService.WepAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.WepAPI.Database;

public class ApplicationDBContext : DbContext
{
    public const string DATABASE_CONFIGURATION = "PostgresDB";

    private readonly string _connectionString;

    public ApplicationDBContext(
        string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Box> Boxes { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

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
        modelBuilder.HasDefaultSchema("inventory_service");

        var boxBuilder = modelBuilder.Entity<Box>();
        boxBuilder.ToTable("boxes");

        boxBuilder.HasKey(b => b.Id);
        boxBuilder.Property(b => b.Id)
            .HasColumnName("id");

        boxBuilder.Property(b => b.Size)
            .IsRequired()
            .HasColumnName("size");

        boxBuilder.Property(b => b.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        var reservationBuilder = modelBuilder.Entity<Reservation>();
        reservationBuilder.ToTable("reservations");

        reservationBuilder.HasKey(r => r.Id);
        reservationBuilder.Property(r => r.Id)
            .HasColumnName("id");

        reservationBuilder.Property(r => r.OrderId)
            .IsRequired()
            .HasColumnName("order_id");

        reservationBuilder.HasOne<Box>()
            .WithMany()
            .HasForeignKey(r => r.BoxId)
            .IsRequired();

        reservationBuilder.Property(inner => inner.StartDate)
                .HasConversion(
                    src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                    dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc))
                .IsRequired()
                .HasColumnName("start_date");

        reservationBuilder.Property(inner => inner.EndDate)
            .HasConversion(
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc))
            .IsRequired()
            .HasColumnName("end_date");

        base.OnModelCreating(modelBuilder);
    }
}
