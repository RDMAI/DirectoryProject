﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.LogTo(Console.WriteLine);
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
