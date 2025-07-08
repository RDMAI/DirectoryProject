using Core.Abstractions;
using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
using DirectoryProject.FileService.Communication;
using DirectoryProject.FileService.Contracts.Requests;
using DirectoryProject.FileService.Contracts.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedKernel;
using SharedKernel.ValueObjects;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.StartUploadLogo;

public class StartUploadLogoHandler
    : ICommandHandler<StartUploadLogoCommand, MultipartStartUploadResponse>
{
    private readonly AbstractValidator<StartUploadLogoCommand> _validator;
    private readonly ILogger<StartUploadLogoHandler> _logger;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IFileService _fileService;

    public StartUploadLogoHandler(
        AbstractValidator<StartUploadLogoCommand> validator,
        ILogger<StartUploadLogoHandler> logger,
        IDepartmentRepository departmentRepository,
        IFileService fileService)
    {
        _validator = validator;
        _logger = logger;
        _departmentRepository = departmentRepository;
        _fileService = fileService;
    }

    public async Task<Result<MultipartStartUploadResponse>> HandleAsync(
        StartUploadLogoCommand command,
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

        // validate command with value object
        var logoResult = Logo.Create(
            fileId: Guid.NewGuid().ToString(),
            location: "photos",
            fileName: command.FileName,
            contentType: command.ContentType);
        if (logoResult.IsFailure)
            return logoResult.Errors;
        var logo = logoResult.Value;

        // check if entity exists
        var entityResult = await _departmentRepository.GetByIdAsync(
            id: Id<Department>.Create(command.DepartmentId),
            cancellationToken: cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Errors;

        // if logo already exist - return error and tell user to delete the old logo first
        if (entityResult.Value.Logo is not null)
        {
            return ErrorHelper.General.MethodNotApplicable(
                "Department already has Logo. To update it, first delete existing Logo");
        }

        var request = new MultipartStartUploadRequest(
            FileName: logo.FileName,
            ContentType: logo.ContentType,
            FileSize: command.Size,
            BucketName: logo.Location);

        var response = await _fileService.MultipartStartUploadAsync(
            request,
            cancellationToken);

        if (response.IsFailure)
            return response.Errors;

        _logger.LogInformation(
            "Multipart upload started. FileId {fileId}, UploadId {uploadId}",
            response.Value.Location.FileId,
            response.Value.UploadId);

        return response;
    }
}
