﻿using DirectoryProject.DirectoryService.Domain;

namespace DirectoryProject.DirectoryService.Application.Shared.DTOs;
public record DepartmentDTO(
    Guid Id,
    string Name,
    Guid? ParentId,
    string Path,
    short Depth,
    int ChildrenCount)
{
    public static DepartmentDTO FromDomainEntity(Department entity)
        => new DepartmentDTO(
            entity.Id.Value,
            entity.Name.Value,
            entity.ParentId?.Value,
            entity.Path.Value,
            entity.Depth,
            entity.ChildrenCount);
}
