using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;

namespace DirectoryProject.DirectoryService.Application.Interfaces;

public interface IPositionRepository
{
    Task<Result<Position>> GetByIdAsync(
        Id<Position> id,
        CancellationToken cancellationToken = default);

    Task<Result<Position>> GetByNameAsync(
        PositionName name,
        CancellationToken cancellationToken = default);

    Task<Result<Position>> CreateAsync(
        Position entity,
        CancellationToken cancellationToken = default);

    Task<Result<Position>> UpdateAsync(
        Position entity,
        CancellationToken cancellationToken = default);

    Task<UnitResult> IsNameUniqueAsync(
        PositionName name,
        CancellationToken cancellationToken = default);
}
