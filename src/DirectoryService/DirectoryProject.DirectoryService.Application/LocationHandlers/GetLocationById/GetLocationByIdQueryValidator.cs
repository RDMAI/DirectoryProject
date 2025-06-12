using DirectoryProject.DirectoryService.Application.Shared.Validation;
using DirectoryProject.DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocationById;

public class GetLocationByIdQueryValidator : AbstractValidator<GetLocationByIdQuery>
{
    public GetLocationByIdQueryValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Id"));
    }
}
