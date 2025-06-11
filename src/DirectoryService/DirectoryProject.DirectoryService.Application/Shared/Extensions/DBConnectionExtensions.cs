using System.Data;
using Dapper;
using DirectoryProject.DirectoryService.Application.Shared.Database;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.Shared.Extensions;

public static class DBConnectionExtensions
{
    public static async Task<T?> QueryFirstOrDefaultAsync<T>(
        this IDbConnection connection,
        CustomSQLBuilder builder,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        string sql = builder.ToString();
        logger?.LogInformation(sql);

        return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
            sql,
            parameters: builder.Parameters,
            cancellationToken: cancellationToken));
    }

    public static async Task<IEnumerable<T>> QueryAsync<T>(
        this IDbConnection connection,
        CustomSQLBuilder builder,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        string sql = builder.ToString();
        logger?.LogInformation(sql);

        return await connection.QueryAsync<T>(new CommandDefinition(
            sql,
            parameters: builder.Parameters,
            cancellationToken: cancellationToken));
    }

    public static async Task<T?> ExecuteScalarAsync<T>(
        this IDbConnection connection,
        CustomSQLBuilder builder,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        string sql = builder.ToString();
        logger?.LogInformation(sql);

        return await connection.ExecuteScalarAsync<T>(new CommandDefinition(
            sql,
            parameters: builder.Parameters,
            cancellationToken: cancellationToken));
    }
}
