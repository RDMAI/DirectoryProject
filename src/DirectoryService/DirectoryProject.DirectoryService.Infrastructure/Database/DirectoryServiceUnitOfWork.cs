using System.Data;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace DirectoryProject.DirectoryService.Infrastructure.Database;
public class DirectoryServiceUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDBContext _dbContext;

    public DirectoryServiceUnitOfWork(ApplicationDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }
}
