using CarAuction.Api.Features.Buyer.Create;
using CarAuction.Api.Features.Buyer.Delete;
using CarAuction.Api.Features.Buyer.Update;
using HotChocolate;

namespace CarAuction.Api.GraphQL;

public class Mutation
{
    private readonly IMediator _mediator;

    public Mutation(IMediator mediator) => _mediator = mediator;

    [GraphQLName("createBuyer")]
    public async Task<BuyerDto> CreateBuyer(string name, int age, string phone, string email)
    {
        var buyer = await _mediator.Send(new CreateBuyerCommand(name, age, phone, email));
        return BuyerDto.FromResponse(buyer);
    }

    [GraphQLName("updateBuyer")]
    public async Task<BuyerDto> UpdateBuyer(Guid id, string name, int age, string phone, string email)
    {
        var buyer = await _mediator.Send(new UpdateBuyerCommand(id, name, age, phone, email));
        return BuyerDto.FromResponse(buyer);
    }

    [GraphQLName("deleteBuyer")]
    public async Task<bool> DeleteBuyer(Guid id)
    {
        await _mediator.Send(new DeleteBuyerCommand(id));
        return true;
    }
}
