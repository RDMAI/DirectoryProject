using Core.Validation;
using FluentValidation;
using SharedKernel;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.DeleteLogo;

public class DeleteLogoCommandValidator : AbstractValidator<DeleteLogoCommand>
{
    public DeleteLogoCommandValidator()
    {
        RuleFor(c => c.DepartmentId)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("DepartmentId"));

        RuleFor(c => c.FileId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsInvalid("FileId"));
    }
}
