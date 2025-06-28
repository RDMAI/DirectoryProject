using Core.Validation;
using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
using SharedKernel;
using FluentValidation;

namespace DirectoryProject.DirectoryService.Application.PositionHandlers.UpdatePosition;
public class UpdatePositionCommandValidator : AbstractValidator<UpdatePositionCommand>
{
    public UpdatePositionCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Id"));

        RuleFor(c => c.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(c => c.Description)
            .MustBeValueObject(PositionDescription.Create);

        RuleFor(c => c.DepartmentIds)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("DepartmentIds"));
    }
}
