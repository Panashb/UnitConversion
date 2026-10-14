using Microsoft.AspNetCore.Mvc;
using UnitConversion.Api.Contracts;
using UnitConversion.Core.Abstractions;
using UnitConversion.Core.Models;

namespace UnitConversion.Api.Controllers;

[ApiController]
[Route("api/v1/units")]
[Produces("application/json")]
public sealed class UnitsController(IUnitRepository repository) : ControllerBase
{
    //Lists supported units, optionally filtered by category.
    [HttpGet]
    [ProducesResponseType<IEnumerable<UnitResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<UnitResponse>> GetUnits([FromQuery] UnitCategory? category)
    {
        if (category is not null && !Enum.IsDefined(category.Value))
        {
            return Problem(
                title: "Invalid category",
                detail: $"Valid categories: {string.Join(", ", Enum.GetNames<UnitCategory>())}.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(repository.GetAll(category)
            .Select(u => new UnitResponse(u.Name, u.Symbol, u.Category.ToString())));
    }
}
