using System.Security.Claims;
using CarAuction.Api.Features.Buyer.Create;
using CarAuction.Api.Features.Buyer.Delete;
using CarAuction.Api.Features.Buyer.GetAll;
using CarAuction.Api.Features.Buyer.GetById;
using CarAuction.Api.Features.Buyer.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.Api.Controllers;

[ApiController]
[Route("api/buyers")]
public class BuyersController : ControllerBase
{
    private readonly IMediator _mediator;
    public BuyersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BuyerResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllBuyersQuery(), cancellationToken);
        return Ok(ApiResponse<IEnumerable<BuyerResponse>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<BuyerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBuyerByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<BuyerResponse>.Ok(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BuyerResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateBuyerCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<BuyerResponse>.Ok(result, "Buyer registered successfully"));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Buyer,Admin")]
    [ProducesResponseType(typeof(ApiResponse<BuyerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBuyerCommand command, CancellationToken cancellationToken)
    {
        var buyerIdClaim = User.FindFirstValue("buyerId");
        if (User.IsInRole("Buyer") && (string.IsNullOrEmpty(buyerIdClaim) || buyerIdClaim != id.ToString()))
            return Forbid();

        var result = await _mediator.Send(command with { Id = id }, cancellationToken);
        return Ok(ApiResponse<BuyerResponse>.Ok(result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteBuyerCommand(id), cancellationToken);
        return NoContent();
    }
}
