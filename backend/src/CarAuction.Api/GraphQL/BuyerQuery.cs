using System.Collections.Generic;
using System.Linq;
using CarAuction.Api.Features.Buyer.GetAll;
using CarAuction.Api.Features.Buyer.GetById;
using HotChocolate;

namespace CarAuction.Api.GraphQL;

public class Query
{
    private readonly IMediator _mediator;

    public Query(IMediator mediator) => _mediator = mediator;

    [GraphQLName("getBuyers")]
    public async Task<List<BuyerDto>> GetBuyers(BuyerFilterInput? filter = null)
    {
        var buyers = await _mediator.Send(new GetAllBuyersQuery(
            FilterAge: filter?.Age,
            FilterName: filter?.Name,
            FilterEmail: filter?.Email));
        return buyers.Select(BuyerDto.FromResponse).ToList();
    }

    [GraphQLName("getBuyer")]
    public async Task<BuyerDto> GetBuyer(Guid id)
    {
        var buyer = await _mediator.Send(new GetBuyerByIdQuery(id));
        return BuyerDto.FromResponse(buyer);
    }
}
