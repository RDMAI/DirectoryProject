using System.Data;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite;
public class DirectoryServiceUnitOfWork : IUnitOfWork
{
    private readonly ApplicationWriteDBContext _dbContext;

    public DirectoryServiceUnitOfWork(ApplicationWriteDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }
}
