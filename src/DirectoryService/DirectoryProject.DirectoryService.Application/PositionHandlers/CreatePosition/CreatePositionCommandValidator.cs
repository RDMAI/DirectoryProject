using DirectoryProject.DirectoryService.Application.Shared.Validation;
using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryProject.DirectoryService.Application.PositionHandlers.CreatePosition;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(c => c.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(c => c.Description)
            .MustBeValueObject(PositionDescription.Create);

        RuleFor(c => c.DepartmentIds)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("DepartmentIds"));
    }
}
