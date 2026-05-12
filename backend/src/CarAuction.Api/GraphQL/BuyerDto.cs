namespace CarAuction.Api.GraphQL;

public sealed record BuyerDto(Guid Id, string Name, int Age, string Phone, string Email)
{
    public static BuyerDto FromResponse(CarAuction.Api.Features.Buyer.Create.BuyerResponse response) => new(
        response.Id,
        response.Name,
        response.Age,
        response.Phone,
        response.Email);
}
