using System.Data;

namespace DirectoryProject.DirectoryService.Application.Shared.Interfaces;

public interface IUnitOfWork
{
    Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default);
}
