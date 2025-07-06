using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Framework;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : Controller
{
    [NonAction]
    public IActionResult ToAPIResponse<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(Envelope.Ok(result.Value));

        return Error(result.Errors);
    }

    [NonAction]
    public IActionResult ToAPIResponse(UnitResult result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(Envelope.Ok(string.Empty));

        return Error(result.Errors);
    }

    [NonAction]
    public IActionResult Error(Error error) => Error([error]);

    [NonAction]
    public IActionResult Error(List<Error>? errors)
    {
        if (errors == null || errors.Count == 0)
        {
            return new ObjectResult(string.Empty)
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }

        List<ResponseError> responseErrors = [];
        List<int> statusCodes = [];
        foreach (var e in errors)
        {
            var statusCode = e.Type switch
            {
                ErrorType.AccessDenied => StatusCodes.Status401Unauthorized,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Failure => StatusCodes.Status500InternalServerError,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError,
            };

            // if status codes do not match - return 500
            if (statusCodes.Count > 0 && statusCodes.Last() != statusCode)
            {
                return new ObjectResult(string.Empty)
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                };
            }

            statusCodes.Add(statusCode);

            responseErrors.Add(new(e.Code, e.Message, e.InvalidField));
        }

        var envelope = Envelope.Error(responseErrors);

        return new ObjectResult(envelope)
        {
            StatusCode = statusCodes[0],
        };
    }
}
