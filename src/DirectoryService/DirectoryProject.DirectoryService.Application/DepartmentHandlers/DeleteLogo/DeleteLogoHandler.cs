using Core.Abstractions;
using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.FileService.Communication;
using DirectoryProject.FileService.Contracts.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedKernel;
using SharedKernel.ValueObjects;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.DeleteLogo;

public class DeleteLogoHandler
    : ICommandHandler<DeleteLogoCommand, DeleteFileResponse>
{
    private readonly AbstractValidator<DeleteLogoCommand> _validator;
    private readonly ILogger<DeleteLogoHandler> _logger;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IFileService _fileService;

    public DeleteLogoHandler(
        AbstractValidator<DeleteLogoCommand> validator,
        ILogger<DeleteLogoHandler> logger,
        IDepartmentRepository departmentRepository,
        IFileService fileService)
    {
        _validator = validator;
        _logger = logger;
        _departmentRepository = departmentRepository;
        _fileService = fileService;
    }

    public async Task<Result<DeleteFileResponse>> HandleAsync(
        DeleteLogoCommand command,
        CancellationToken cancellationToken = default)
    {
        // command validation
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validatorResult.IsValid)
        {
            return validatorResult.Errors
                .Select(e => Error.Deserialize(e.ErrorMessage))
                .ToList();
        }

        // check if entity exists
        var entityResult = await _departmentRepository.GetByIdAsync(
            id: Id<Department>.Create(command.DepartmentId),
            cancellationToken: cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Errors;

        var entity = entityResult.Value;

        if (entity.Logo is null)
            return new DeleteFileResponse(string.Empty);

        var fileResponse = await _fileService.DeleteFileAsync(
            entity.Logo.FileId,
            entity.Logo.Location,
            cancellationToken);
        if (fileResponse.IsFailure)
            return fileResponse.Errors;

        entity.UpdateLogo(null);
        var updateResponse = await _departmentRepository.UpdateAsync(entity, cancellationToken);
        if (updateResponse.IsFailure)
            return updateResponse.Errors;

        return fileResponse;
    }
}
