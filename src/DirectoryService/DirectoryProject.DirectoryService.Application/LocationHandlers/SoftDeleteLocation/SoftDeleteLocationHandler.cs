using DirectoryProject.DirectoryService.Application.Interfaces;
using DirectoryProject.DirectoryService.Application.Shared.Interfaces;
using DirectoryProject.DirectoryService.Domain;
using DirectoryProject.DirectoryService.Domain.Shared;
using DirectoryProject.DirectoryService.Domain.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.SoftDeleteLocation;

public class SoftDeleteLocationHandler
    : ICommandHandler<SoftDeleteLocationCommand>
{
    private readonly AbstractValidator<SoftDeleteLocationCommand> _validator;
    private readonly ILogger<SoftDeleteLocationHandler> _logger;
    private readonly ILocationRepository _locationRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteLocationHandler(
        AbstractValidator<SoftDeleteLocationCommand> validator,
        ILogger<SoftDeleteLocationHandler> logger,
        ILocationRepository locationRepository,
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _logger = logger;
        _locationRepository = locationRepository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UnitResult> HandleAsync(
        SoftDeleteLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        // command validation
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validatorResult.IsValid)
        {
            return validatorResult.Errors
                .Select(e => Error.Deserialize(e.ErrorMessage))
                .ToList();
        }

        // check if entity exists
        var entityResult = await _locationRepository.GetByIdAsync(
            id: Id<Location>.Create(command.Id),
            cancellationToken: cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Errors;

        var entity = entityResult.Value;

        var departmentsResult = await _departmentRepository.GetDepartmentsForLocationAsync(
            entity.Id,
            cancellationToken);
        if (departmentsResult.IsFailure)
            return departmentsResult.Errors;
        if (departmentsResult.Value.Any())
        {
            return ErrorHelper.Database.DeleteFailedConflict(
                $"Failed to delete Location with id {entity.Id.Value}. It still has active departments");
        }

        entity.Deactivate();
        var entityUpdateResult = await _locationRepository.UpdateAsync(
            entity,
            cancellationToken);
        if (entityUpdateResult.IsFailure)
            return entityUpdateResult.Errors;

        _logger.LogInformation("Location with id {0} was deactivated", entity.Id);

        return UnitResult.Success();
    }
}
