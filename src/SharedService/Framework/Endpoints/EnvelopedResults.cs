using Microsoft.AspNetCore.Http;
using SharedKernel;

namespace Framework.Endpoints;

public static class EnvelopedResults
{
    public static IResult Ok(object? value) => Results.Ok(Envelope.Ok(value));

    public static IResult Error(List<Error> errors)
    {
        if (errors == null || errors.Count == 0)
            return Results.StatusCode(StatusCodes.Status500InternalServerError);

        // define status code for response
        var errorTypes = errors.Select(e => e.Type).Distinct();
        if (errorTypes.Count() > 1)
            return Results.StatusCode(StatusCodes.Status500InternalServerError);

        var envelope = Envelope.Error(errors.Select(e => new ResponseError(e.Code, e.Message, e.InvalidField)));

        return errorTypes.First() switch
        {
            ErrorType.AccessDenied => Results.Unauthorized(),
            ErrorType.Conflict => Results.Conflict(envelope),
            ErrorType.Failure => Results.StatusCode(StatusCodes.Status500InternalServerError),
            ErrorType.NotFound => Results.NotFound(envelope),
            ErrorType.Validation => Results.BadRequest(envelope),
            _ => Results.StatusCode(StatusCodes.Status500InternalServerError),
        };
    }
}
