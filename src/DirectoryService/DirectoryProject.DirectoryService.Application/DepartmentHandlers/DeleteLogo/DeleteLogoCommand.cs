using Core.Abstractions;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.DeleteLogo;

public record DeleteLogoCommand(
    Guid DepartmentId,
    string FileId) : ICommand;
