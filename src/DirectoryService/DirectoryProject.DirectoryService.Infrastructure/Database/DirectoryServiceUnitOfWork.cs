using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using System.Data;

namespace DirectoryProject.DirectoryService.Infrastructure.Database;
public class DirectoryServiceUnitOfWork : IUnitOfWork
{
    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {

    }
}
