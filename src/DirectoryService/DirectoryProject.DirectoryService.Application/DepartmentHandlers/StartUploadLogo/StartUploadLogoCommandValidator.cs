using Core.Validation;
using FluentValidation;
using SharedKernel;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.StartUploadLogo;

public class StartUploadLogoCommandValidator : AbstractValidator<StartUploadLogoCommand>
{
    public StartUploadLogoCommandValidator()
    {
        RuleFor(c => c.DepartmentId)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("DepartmentId"));

        RuleFor(c => c.Size)
            .Must(s => s > 0)
            .WithError(ErrorHelper.General.ValueIsInvalid("Size"));
    }
}
