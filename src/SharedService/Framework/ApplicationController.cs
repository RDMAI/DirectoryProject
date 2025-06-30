using Framework.Helpers;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Framework;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : Controller
{
    [NonAction]
    public IActionResult ToAPIResponse<T>(Result<T> result) => APIResponseHelper.ToAPIResponse(result);

    [NonAction]
    public IActionResult ToAPIResponse(UnitResult result) => APIResponseHelper.ToAPIResponse(result);

    [NonAction]
    public IActionResult Error(Error error) => APIResponseHelper.Error(error);

    [NonAction]
    public IActionResult Error(List<Error>? errors) => APIResponseHelper.Error(errors);
}
