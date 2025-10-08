using Chirp.Razor;
using Xunit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace HTTPTest
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var cheepService = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ICheepService));

                if (cheepService != null)
                {
                    services.Remove(cheepService);
                }
                services.AddSingleton<ICheepService, CheepService>();
                services.AddSingleton<IDBFacade, StubDBFacade>();
            });
        }
    }
    public class HTTPTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public HTTPTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public void GetEndpointsReturnSucces()
        {

        }
    }
}