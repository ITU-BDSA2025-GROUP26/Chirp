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
                var dbContext = services.SingleOrDefault(
                    d => d.ServiceType ==
                    typeof(DBFacade));
                if (dbContext != null) { services.Remove(dbContext); }
                ;

                services.AddSingleton<IDBFacade, StubDBFacade>();
            });
            builder.UseEnvironment("Development");
        }
    }
    public class HTTPTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        public HTTPTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetEndpointsReturnSuccess()
        {
            //Arrange
            var response = await _client.GetAsync("/");

            //Act
            var result = response.IsSuccessStatusCode;

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ChirpPageContainsDummyCheeps()
        {
            //Arrange
            var response = await _client.GetAsync("/");

            //Act
            var list = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Contains("Bo", list);
            Assert.Contains("Anne", list);
            Assert.Contains("Hej Anne", list);
            Assert.Contains("Hej Bo", list);
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}