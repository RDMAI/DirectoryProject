using System.Data;

namespace DirectoryProject.SharedService.Core.Database;

public interface IUnitOfWork
{
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
