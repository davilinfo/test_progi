namespace CarAuction.Api.GraphQL;

[GraphQLName("BuyerFilter")]
public record BuyerFilterInput(int? Age = null, string? Name = null, string? Email = null);
