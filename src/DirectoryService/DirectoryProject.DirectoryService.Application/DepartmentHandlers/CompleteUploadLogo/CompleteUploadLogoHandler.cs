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

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.CompleteUploadLogo;

public class CompleteUploadLogoHandler
    : ICommandHandler<CompleteUploadLogoCommand, CompleteMultipartUploadResponse>
{
    private readonly AbstractValidator<CompleteUploadLogoCommand> _validator;
    private readonly ILogger<CompleteUploadLogoHandler> _logger;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IFileService _fileService;

    public CompleteUploadLogoHandler(
        AbstractValidator<CompleteUploadLogoCommand> validator,
        ILogger<CompleteUploadLogoHandler> logger,
        IDepartmentRepository departmentRepository,
        IFileService fileService)
    {
        _validator = validator;
        _logger = logger;
        _departmentRepository = departmentRepository;
        _fileService = fileService;
    }

    public async Task<Result<CompleteMultipartUploadResponse>> HandleAsync(
        CompleteUploadLogoCommand command,
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

        var request = new CompleteMultipartUploadRequest(
            Location: new(command.FileId, "photos"),
            UploadId: command.UploadId,
            PartETags: command.PartETags);

        var response = await _fileService.MultipartCompleteUploadAsync(
            request,
            cancellationToken);

        if (response.IsFailure)
            return response.Errors;

        var logo = Logo.Create(
            fileId: request.Location.FileId,
            location: request.Location.BucketName,
            fileName: response.Value.Metadata.FileName,
            contentType: response.Value.Metadata.ContentType).Value;

        entity.UpdateLogo(logo);

        var updateResponse = await _departmentRepository.UpdateAsync(entity, cancellationToken);
        if (updateResponse.IsFailure)
            return updateResponse.Errors;

        return response;
    }
}
