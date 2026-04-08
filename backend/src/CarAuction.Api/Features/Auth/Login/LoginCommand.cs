namespace CarAuction.Api.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

public record LoginResponse(string Token, UserDto User);

public record UserDto(Guid Id, string Email, UserRole Role, Guid? BuyerId, Guid? SellerId);
