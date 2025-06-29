using Core.Validation;
using SharedKernel;
using FluentValidation;

namespace DirectoryProject.DirectoryService.Application.DepartmentHandlers.GetRootDepartments;

public class GetRootDepartmentsQueryValidator : AbstractValidator<GetRootDepartmentsQuery>
{
    public GetRootDepartmentsQueryValidator()
    {
        RuleFor(q => q.Page)
            .Must(d => d > 0)
            .WithError(ErrorHelper.General.ValueIsInvalid(nameof(GetRootDepartmentsQuery.Page)));

        RuleFor(q => q.Size)
            .Must(d => d > 0 && d < 1000)
            .WithError(ErrorHelper.General.ValueIsInvalid(nameof(GetRootDepartmentsQuery.Size)));

        RuleFor(q => q.Prefetch)
            .Must(d => d > 0 && d < 3)
            .WithError(ErrorHelper.General.ValueIsInvalid(nameof(GetRootDepartmentsQuery.Prefetch)));
    }
}
