using CarAuction.Api.Features.Fee.Calculate;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.Api.Controllers;

[ApiController]
[Route("api/fees")]
public class FeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FeesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Calculates all fees for a given vehicle base price and type.</summary>
    [HttpGet("calculate")]
    [ProducesResponseType(typeof(ApiResponse<FeeCalculationResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Calculate(
        [FromQuery] decimal basePrice,
        [FromQuery] VehicleType vehicleType,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CalculateFeeQuery(basePrice, vehicleType), cancellationToken);
        return Ok(ApiResponse<FeeCalculationResult>.Ok(result));
    }
}
