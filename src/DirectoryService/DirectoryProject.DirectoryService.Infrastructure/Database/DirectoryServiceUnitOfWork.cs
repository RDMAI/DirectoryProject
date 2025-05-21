using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using System.Data;

namespace DirectoryProject.DirectoryService.Infrastructure.Database;
public class DirectoryServiceUnitOfWork : IUnitOfWork
{
    public async Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {

    }
}
