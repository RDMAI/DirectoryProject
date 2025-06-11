using System.Data;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using Npgsql;

namespace DirectoryProject.DirectoryService.Infrastructure.DatabaseRead;

public class ReadDBConnectionFactory : IDBConnectionFactory
{
    private readonly string _connectionString;

    public ReadDBConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Create()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
