using System.Security.Claims;
using CarAuction.Api.Features.Seller.Create;
using CarAuction.Api.Features.Seller.Delete;
using CarAuction.Api.Features.Seller.GetAll;
using CarAuction.Api.Features.Seller.GetById;
using CarAuction.Api.Features.Seller.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.Api.Controllers;

[ApiController]
[Route("api/sellers")]
public class SellersController : ControllerBase
{
    private readonly IMediator _mediator;
    public SellersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SellerResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllSellersQuery(), cancellationToken);
        return Ok(ApiResponse<IEnumerable<SellerResponse>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<SellerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSellerByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<SellerResponse>.Ok(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SellerResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateSellerCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<SellerResponse>.Ok(result, "Seller registered successfully"));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Seller,Admin")]
    [ProducesResponseType(typeof(ApiResponse<SellerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSellerCommand command, CancellationToken cancellationToken)
    {
        var sellerIdClaim = User.FindFirstValue("sellerId");
        if (User.IsInRole("Seller") && (string.IsNullOrEmpty(sellerIdClaim) || sellerIdClaim != id.ToString()))
            return Forbid();

        var result = await _mediator.Send(command with { Id = id }, cancellationToken);
        return Ok(ApiResponse<SellerResponse>.Ok(result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSellerCommand(id), cancellationToken);
        return NoContent();
    }
}
