using Core.Validation;
using FluentValidation;
using SharedKernel;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.CompleteUploadLogo;

public class CompleteUploadLogoCommandValidator : AbstractValidator<CompleteUploadLogoCommand>
{
    public CompleteUploadLogoCommandValidator()
    {
        RuleFor(c => c.DepartmentId)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("DepartmentId"));

        RuleFor(c => c.FileId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("FileId"));

        RuleFor(c => c.UploadId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("UploadId"));
    }
}
