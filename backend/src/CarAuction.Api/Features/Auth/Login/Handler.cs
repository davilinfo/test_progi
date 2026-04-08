using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarAuction.Api.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CarAuction.Api.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(AppDbContext context, IConfiguration configuration, ILogger<LoginHandler> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for {Email}", request.Email);

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null || user.PasswordHash != DataSeeder.HashPassword(request.Password))
        {
            _logger.LogWarning("Invalid credentials for {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var token = GenerateToken(user);
        _logger.LogInformation("Login successful for {Email} with role {Role}", user.Email, user.Role);

        return new LoginResponse(token, new UserDto(user.Id, user.Email, user.Role, user.BuyerId, user.SellerId));
    }

    private string GenerateToken(User user)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JWT secret key is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("buyerId", user.BuyerId?.ToString() ?? string.Empty),
            new Claim("sellerId", user.SellerId?.ToString() ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expMinutes = int.TryParse(_configuration["JwtSettings:ExpirationMinutes"], out var m) ? m : 480;

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
