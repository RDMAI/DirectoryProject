using DirectoryProject.DirectoryService.Application.Shared.Validation;
using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
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
    }
}
