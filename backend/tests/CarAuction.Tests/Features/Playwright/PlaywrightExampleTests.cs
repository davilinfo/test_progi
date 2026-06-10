using System.Text.Json;
using System.Text.Json.Serialization;
using CarAuction.Api.Features.Auth.Login;
using CarAuction.Api.Shared.Model;
using Microsoft.Playwright;
using Xunit;

namespace CarAuction.Tests.Features.PlaywrightExample;

public class PlaywrightExampleTests : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public async Task InitializeAsync()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.DisposeAsync();
        }

        _playwright?.Dispose();
    }

    [Fact]
    public async Task HomePage_Should_Return_Correct_Title()
    {
        var page = await _browser!.NewPageAsync();
        await page.GotoAsync("http://localhost:5173");

        var title = await page.TitleAsync();

        title.Should().Contain("Car Auction");
    }

    [Fact]
    public async Task AuctionsApi_Should_Return_Success_Status()
    {
        await using var apiRequest = await _playwright!.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = "http://localhost:8080"
        });

        var response = await apiRequest.GetAsync("/api/auctions");

        response.Ok.Should().BeTrue();
        response.Headers.Should().ContainKey("content-type");
    }

    [Theory]
    [InlineData("00000000-0000-0000-0005-000000000001", 10000)]
    public async Task AuctionApi_Should_Return_Ok_Post_Auction_Value(string auctionId, decimal value)
    {
        await using var apiRequest = await _playwright!.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = "http://localhost:8080",

        });

        var responseAuth = await apiRequest.PostAsync($"/api/auth/login", new APIRequestContextOptions
        {
            DataObject = new
            {
                email = "buyer1@carauction.com",
                password = "Password123!"
            }
        });

        var authText = await responseAuth.TextAsync();
        var authPayload = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(authText, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });
        var loginResponse = authPayload?.Data;

        loginResponse.Should().NotBeNull();

        var response = await apiRequest.PostAsync($"/api/auctions/{auctionId}/bid", new APIRequestContextOptions
        {
            DataObject = new
            { 
                BidAmount = value
            },
            Headers = new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {loginResponse!.Token}" }
            }
        });

        response.Ok.Should().BeTrue();
    }
}
