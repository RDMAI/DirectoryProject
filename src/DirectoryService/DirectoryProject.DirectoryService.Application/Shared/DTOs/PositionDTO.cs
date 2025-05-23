using DirectoryProject.DirectoryService.Domain;

namespace DirectoryProject.DirectoryService.Application.Shared.DTOs;

public record PositionDTO(
    Guid Id,
    string Name,
    string Description)
{
    public static PositionDTO FromDomainEntity(Position entity)
        => new PositionDTO(
            Id: entity.Id.Value,
            Name: entity.Name.Value,
            Description: entity.Description.Value);
}
