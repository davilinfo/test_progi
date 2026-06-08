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
}
