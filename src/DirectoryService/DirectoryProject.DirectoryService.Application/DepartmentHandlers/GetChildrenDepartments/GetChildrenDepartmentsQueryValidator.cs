using DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetRootDepartments;
using DirectoryProject.DirectoryService.Application.Shared.Validation;
using DirectoryProject.DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetChildrenDepartments;

public class GetChildrenDepartmentsQueryValidator : AbstractValidator<GetChildrenDepartmentsQuery>
{
    public GetChildrenDepartmentsQueryValidator()
    {
        RuleFor(q => q.ParentId)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("ParentId"));

        RuleFor(q => q.Page)
            .Must(d => d > 0)
            .WithError(ErrorHelper.General.ValueIsInvalid(nameof(GetRootDepartmentsQuery.Page)));

        RuleFor(q => q.Size)
            .Must(d => d > 0 && d < 1000)
            .WithError(ErrorHelper.General.ValueIsInvalid(nameof(GetRootDepartmentsQuery.Size)));
    }
}
