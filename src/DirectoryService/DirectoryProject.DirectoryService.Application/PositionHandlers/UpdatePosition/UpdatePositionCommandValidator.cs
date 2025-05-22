using DirectoryProject.DirectoryService.Application.Shared.Validation;
using DirectoryProject.DirectoryService.Domain.PositionValueObjects;
using DirectoryProject.DirectoryService.Domain.Shared;
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
    }
}
