using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Application.Shared.Extensions;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.Shared;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.GetLocations;

public class GetLocationsHandler
    : IQueryHandler<GetLocationsQuery, FilteredListDTO<LocationDTO>>
{
    private readonly AbstractValidator<GetLocationsQuery> _validator;
    private readonly ILogger<GetLocationsHandler> _logger;
    private readonly ILocationRepository _locationRepository;

    public GetLocationsHandler(
        AbstractValidator<GetLocationsQuery> validator,
        ILogger<GetLocationsHandler> logger,
        ILocationRepository locationRepository)
    {
        _validator = validator;
        _logger = logger;
        _locationRepository = locationRepository;
    }

    public async Task<Result<FilteredListDTO<LocationDTO>>> HandleAsync(
        GetLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        // command validation
        var validatorResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validatorResult.IsValid)
        {
            return validatorResult.Errors
                .Select(e => Error.Deserialize(e.ErrorMessage))
                .ToList();
        }

        var queryFilter = (IQueryable<Location> locations) =>
        {
            // .Where(d => EF.Functions.ILike((string)d.Name, $"%{query.Search}%"));
            if (string.IsNullOrEmpty(query.Search) == false)
                locations = locations.ILike(d => d.Name, query.Search);

            return locations;
        };

        var result = await _locationRepository.GetAsync(
            queryFilter,
            query.Page,
            query.Size,
            cancellationToken);
        if (result.IsFailure)
            return result.Errors;

        var resultList = result.Value.Values;

        return new FilteredListDTO<LocationDTO>(
            query.Page,
            query.Size,
            resultList.Select(LocationDTO.FromDomainEntity),
            result.Value.TotalCount);
    }
}
