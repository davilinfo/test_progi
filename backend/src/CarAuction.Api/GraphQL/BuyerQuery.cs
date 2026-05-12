using System.Collections.Generic;
using System.Linq;
using CarAuction.Api.Features.Buyer.GetAll;
using CarAuction.Api.Features.Buyer.GetById;
using HotChocolate;

namespace CarAuction.Api.GraphQL;

public class Query
{
    [GraphQLName("getBuyers")]
    public async Task<List<BuyerDto>> GetBuyers([Service] IMediator mediator, BuyerFilterInput? filter = null)
    {
        var buyers = await mediator.Send(new GetAllBuyersQuery(
            FilterAge: filter?.Age,
            FilterName: filter?.Name,
            FilterEmail: filter?.Email));
        return buyers.Select(BuyerDto.FromResponse).ToList();
    }

    [GraphQLName("getBuyer")]
    public async Task<BuyerDto> GetBuyer([Service] IMediator mediator, Guid id)
    {
        var buyer = await mediator.Send(new GetBuyerByIdQuery(id));
        return BuyerDto.FromResponse(buyer);
    }
}
