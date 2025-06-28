using Core.Validation;
using SharedKernel;
using FluentValidation;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocations;

public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator()
    {
        RuleFor(q => q.Page)
            .Must(d => d > 0)
            .WithError(ErrorHelper.General.ValueIsInvalid(nameof(GetLocationsQuery.Page)));

        RuleFor(q => q.Size)
            .Must(d => d > 0 && d < 1000)
            .WithError(ErrorHelper.General.ValueIsInvalid(nameof(GetLocationsQuery.Size)));
    }
}
