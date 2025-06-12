using DirectoryProject.DirectoryService.Domain.Shared;

namespace DirectoryProject.DirectoryService.Application.Shared.Interfaces;

public interface IQueryHandler<TQuery, TReturnType> where TQuery : IQuery
{
    public Task<Result<TReturnType>> HandleAsync(
        TQuery query,
        CancellationToken cancellationToken = default);
}
