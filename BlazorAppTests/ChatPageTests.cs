namespace Abotti.BlazorAppTests;

[Collection(PlaywrightFixture.PlaywrightCollection)]
public class ChatPageTests
{
    private readonly PlaywrightFixture playwrightFixture;

    public ChatPageTests(PlaywrightFixture playwrightFixture)
    {
        this.playwrightFixture = playwrightFixture;
    }


    // [Fact]
    // [Trait("Category", "PlaywrightTest")]
    // public async Task ShouldRenderPageCorrectly()
    // {
    //     var url = "http://127.0.0.1:5000";
    //
    //
    //     // Create the host factory with the App class as parameter and the
    //     // url we are going to use.
    //     using var hostFactory = new WebTestingHostFactory<Abotti.BlazorApp>();
    //
    //     hostFactory
    //         // Override host configuration to mock stuff if required.
    //         .WithWebHostBuilder(builder =>
    //         {
    //             // Setup the url to use.
    //             builder.UseUrls(url);
    //             // Replace or add services if needed.
    //             builder.ConfigureServices(services =>
    //                 {
    //                     // services.AddTransient<....>();
    //                 })
    //                 // Replace or add configuration if needed.
    //                 .ConfigureAppConfiguration((app, conf) =>
    //                 {
    //                     // conf.AddJsonFile("appsettings.Test.json");
    //                 });
    //         })
    //         // Create the host using the CreateDefaultClient method.
    //         .CreateDefaultClient();
    //
    //     await playwrightFixture.GotoPageAsync(
    //         url,
    //         async (page) =>
    //         {
    //             // Apply the test logic on the given page.
    //             await page.GotoAsync($"{url}/Chat");
    //             await page.Locator("h3:has-text(\"Chat\")").IsVisibleAsync();
    //         },
    //         Browser.Chromium);
    // }
}