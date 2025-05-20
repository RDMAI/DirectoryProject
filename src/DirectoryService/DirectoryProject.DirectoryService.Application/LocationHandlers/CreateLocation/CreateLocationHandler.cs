using DirectoryProject.DirectoryService.Application.Shared.DTOs;
using DirectoryProject.DirectoryService.Domain.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryProject.DirectoryService.Application.LocationHandlers.CreateLocation;

public class CreateLocationHandler
{
    private readonly AbstractValidator<CreateLocationCommand> _validator;
    private readonly ILogger<CreateLocationHandler> _logger;
    private readonly 

    public CreateLocationHandler(
        AbstractValidator<CreateLocationCommand> validator,
        ILogger<CreateLocationHandler> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<LocationDTO>> HandleAsync(
        CreateLocationCommand command,
        CancellationToken cancellationToken)
    {
        // command validation
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validatorResult.IsValid)
        {
            return validatorResult.Errors
                .Select(e => Error.Deserialize(e.ErrorMessage))
                .ToList();
        }


    }
}
