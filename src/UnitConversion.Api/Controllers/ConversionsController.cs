using Microsoft.AspNetCore.Mvc;
using UnitConversion.Api.Contracts;
using UnitConversion.Core.Abstractions;

namespace UnitConversion.Api.Controllers;

[ApiController]
[Route("api/v1/conversions")]
[Produces("application/json")]
public sealed class ConversionsController(IConversionService conversionService) : ControllerBase
{
    //Converts a number from one unit to another within a category.
    //Sample Request: GET /api/v1/conversions?category=length&amp;from=meter&amp;to=foot&amp;value=10
    [HttpGet]
    [ProducesResponseType<ConversionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<ConversionResponse> Convert([FromQuery] ConvertRequest request)
    {
        var result = conversionService.Convert(
            request.Category!.Value, request.From!, request.To!, request.Value!.Value);

        return Ok(new ConversionResponse(
            result.Category.ToString(),
            result.From.Name,
            result.To.Name,
            result.InputValue,
            result.Result));
    }
}
