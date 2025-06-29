using DirectoryProject.SharedService.SharedKernel;

namespace DirectoryProject.SharedService.Core.Abstractions;

public interface IQueryHandler<TQuery, TReturnType> where TQuery : IQuery
{
    public Task<Result<TReturnType>> HandleAsync(
        TQuery query,
        CancellationToken cancellationToken = default);
}
