using System.Security.Claims;
using CarAuction.Api.Features.Auction.Close;
using CarAuction.Api.Features.Auction.Create;
using CarAuction.Api.Features.Auction.GetAll;
using CarAuction.Api.Features.Auction.GetById;
using CarAuction.Api.Features.Auction.PlaceBid;
using CarAuction.Api.Shared.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.Api.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuctionsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AuctionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] AuctionStatus? status, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllAuctionsQuery(status), cancellationToken);
        return Ok(ApiResponse<IEnumerable<AuctionResponse>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AuctionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAuctionByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<AuctionResponse>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Seller,Admin")]
    [ProducesResponseType(typeof(ApiResponse<AuctionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAuctionCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<AuctionResponse>.Ok(result, "Auction created successfully"));
    }

    [HttpPost("{id:guid}/bid")]
    [Authorize(Roles = "Buyer")]
    [ProducesResponseType(typeof(ApiResponse<AuctionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PlaceBid(Guid id, [FromBody] PlaceBidRequest request, CancellationToken cancellationToken)
    {
        var buyerIdClaim = User.FindFirstValue("buyerId");
        if (string.IsNullOrEmpty(buyerIdClaim) || !Guid.TryParse(buyerIdClaim, out var buyerId))
            return Forbid();

        var result = await _mediator.Send(new PlaceBidCommand(id, buyerId, request.BidAmount), cancellationToken);
        return Ok(ApiResponse<AuctionResponse>.Ok(result, "Bid placed successfully"));
    }

    [HttpPost("{id:guid}/close")]
    [Authorize(Roles = "Seller,Admin")]
    [ProducesResponseType(typeof(ApiResponse<CloseAuctionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Close(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CloseAuctionCommand(id), cancellationToken);
        return Ok(ApiResponse<CloseAuctionResponse>.Ok(result, "Auction closed successfully"));
    }
}

public record PlaceBidRequest(decimal BidAmount);
