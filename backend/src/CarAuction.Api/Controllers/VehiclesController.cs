using CarAuction.Api.Features.Vehicle.Create;
using CarAuction.Api.Features.Vehicle.Delete;
using CarAuction.Api.Features.Vehicle.GetAll;
using CarAuction.Api.Features.Vehicle.GetById;
using CarAuction.Api.Features.Vehicle.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.Api.Controllers;

[ApiController]
[Route("api/vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VehiclesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<VehicleResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllVehiclesQuery(), cancellationToken);
        return Ok(ApiResponse<IEnumerable<VehicleResponse>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<VehicleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetVehicleByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<VehicleResponse>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Seller,Admin")]
    [ProducesResponseType(typeof(ApiResponse<VehicleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateVehicleCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<VehicleResponse>.Ok(result, "Vehicle created successfully"));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Seller,Admin")]
    [ProducesResponseType(typeof(ApiResponse<VehicleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command with { Id = id }, cancellationToken);
        return Ok(ApiResponse<VehicleResponse>.Ok(result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Seller,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteVehicleCommand(id), cancellationToken);
        return NoContent();
    }
}
