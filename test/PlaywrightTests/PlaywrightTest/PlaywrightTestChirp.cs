using Microsoft.Playwright;

namespace PlaywrightTest;

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

    [Test]
    public async Task UsersCanRegisterNewAccount()
    {
        await Page.GotoAsync("http://localhost:5273/");
        
        await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));
        
        var register = Page.Locator("text=register");
        
        await register.ClickAsync();
        
        await Expect(Page).ToHaveURLAsync(new Regex("/Account/Register"));

        await Expect(Page).ToHaveTitleAsync(new Regex("Register"));

        await Page.GetByLabel("Email").FillAsync("nanna@test.com");
        await Page.GetByLabel("Username").FillAsync("test");
        await Page.GetByLabel("Password", new (){Exact = true}).FillAsync("I!s3456");
        await Page.GetByLabel("Confirm Password", new (){Exact = true}).FillAsync("I!s3456");
    }

    [Test]
    public async Task UserCanLoginAndSeeDashboard()
    {
        await Page.GotoAsync("http://localhost:5273/");
        
        await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));
        
        var login = Page.Locator("text=login");

        await login.ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/Account/Login"));
        
        await Page.GetByLabel("Username").FillAsync("HelgeCPH");
        await Page.GetByLabel("Password", new (){Exact = true}).FillAsync("LetM31n!");
        
        var loginButton = Page.Locator("id=login-submit");
        
        await loginButton.ClickAsync();
        
        await Expect(Page).ToHaveURLAsync(new Regex("/"));
    }
}