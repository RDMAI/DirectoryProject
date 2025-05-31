using DirectoryProject.DirectoryService.Application.Shared.Validation;
using DirectoryProject.DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryProject.DirectoryService.Application.PositionHandlers.SoftDeletePosition;

public class SoftDeletePositionCommandValidator : AbstractValidator<SoftDeletePositionCommand>
{
    public SoftDeletePositionCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Id"));
    }
}
