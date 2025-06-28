using Core.Validation;
using DirectoryProject.DirectoryService.Application.DTOs;
using DirectoryProject.DirectoryService.Domain.LocationValueObjects;
using SharedKernel;
using FluentValidation;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.CreateLocation;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(c => c.Name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(c => c.Address)
            .MustBeValueObject(AddressFromDTO);

        RuleFor(c => c.TimeZone)
            .MustBeValueObject(IANATimeZone.Create);
    }

    private static Result<LocationAddress> AddressFromDTO(AddressDTO dto)
        => LocationAddress.Create(dto.City, dto.Street, dto.HouseNumber);
}
