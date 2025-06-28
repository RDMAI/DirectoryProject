using SharedKernel;

namespace Core.Abstractions;

public interface ICommandHandler<TCommand, TReturnType> where TCommand : ICommand
{
    public Task<Result<TReturnType>> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default);
}

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    public Task<UnitResult> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default);
}
