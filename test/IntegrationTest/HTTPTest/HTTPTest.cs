using Chirp.Razor;
using Xunit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
namespace HTTPTest
{

    public class HTTPTest:IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public HTTPTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public void GetEndpointsReturnSucces()
        {
            var client = _factory.CreateClient();
        }
    }
}