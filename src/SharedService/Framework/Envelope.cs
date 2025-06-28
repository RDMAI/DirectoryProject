namespace Framework;

public record ResponseError(string ErrorCode, string ErrorMessage, string? Field);
public record Envelope
{
    public object? Result { get; }
    public IEnumerable<ResponseError> Errors { get; }
    public DateTime TimeGenerated { get; }

    private Envelope(object? result, IEnumerable<ResponseError> errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.Now;
    }

    public static Envelope Ok(object? result) =>
        new(result, []);

    public static Envelope Error(IEnumerable<ResponseError> errors) =>
        new(null, errors);
}
