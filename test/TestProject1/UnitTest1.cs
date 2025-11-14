using Microsoft.Playwright;

namespace TestProject1;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    [Test]
    public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingtoTheIntroPage()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));

        // create a locator
        var getStarted = Page.Locator("text=Get Started");

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

        // Click the get started link.
        await getStarted.ClickAsync();

        // Expects the URL to contain intro.
        await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
    }

    [Test]
    public async Task HomepageLoadsCorrectlyAndClickingOnLogin()
    {
        await Page.GotoAsync("http://localhost:5273/");

        await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));

        var login = Page.Locator("text=login");

        await login.ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/Account/Login"));
    }

    [Test]
    public async Task HomepageLoadsAndRegisterLinksToRegisterPage()
    {
        await Page.GotoAsync("http://localhost:5273/");

        await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));
        
        var register = Page.Locator("text=register");
        
        await register.ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/Account/Register"));
    }
}