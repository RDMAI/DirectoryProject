using System.Text;

namespace DirectoryProject.DirectoryService.Domain.Shared;

public record Result<TValue>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public TValue Value { get; }

    public List<Error> Errors { get; } = [];

    public static Result<TValue> Success(TValue value)
        => new Result<TValue>(value: value, errors: []);

    public static Result<TValue> Error(List<Error> errors)
        => new Result<TValue>(value: default, errors: errors);

    public static implicit operator Result<TValue>(TValue value) => Result<TValue>.Success(value);
    public static implicit operator Result<TValue>(List<Error> errors) => Result<TValue>.Error(errors);
    public static implicit operator Result<TValue>(Error error) => Result<TValue>.Error([error]);

    // for debugging
    public override string ToString()
    {
        if (IsSuccess)
            return $"Success: {Value!}";

        var sb = new StringBuilder("Failed:");
        foreach (var error in Errors!)
            sb.AppendLine(error.ToString());

        return sb.ToString();
    }

    private Result(TValue value, List<Error> errors)
    {
        Value = value;
        Errors = errors;
    }
}

public record UnitResult
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public List<Error> Errors { get; }

    public static UnitResult Success() => new UnitResult([]);
    public static UnitResult Error(List<Error> errors) => new UnitResult(errors);

    public static implicit operator UnitResult(List<Error> errors) => Error(errors);
    public static implicit operator UnitResult(Error error) => Error([error]);

    // for debugging
    public override string ToString()
    {
        if (IsSuccess)
            return "Success";

        var sb = new StringBuilder("Failed:");
        foreach (var error in Errors!)
            sb.AppendLine(error.ToString());

        return sb.ToString();
    }

    private UnitResult(List<Error> errors)
    {
        Errors = errors;
    }
}
