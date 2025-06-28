using Core.Validation;
using SharedKernel;
using FluentValidation;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.SoftDeleteDepartment;

public class SoftDeleteDepartmentCommandValidator : AbstractValidator<SoftDeleteDepartmentCommand>
{
    public SoftDeleteDepartmentCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Id"));
    }
}
