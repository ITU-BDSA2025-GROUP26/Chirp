using Microsoft.AspNetCore.Builder;
using Chirp.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.ComponentModel;
using System.Data.Common;

namespace Chirp.Web.Tests;

public class CustomWebApplicationFactory: WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbcontext = services.SingleOrDefault(
                d => d.ServiceType ==
                typeof(ChirpDBContext)
            );
            if (dbcontext != null)
            {
                services.Remove(dbcontext);
            }
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                return connection;
            });
            services.AddDbContext<ChirpDBContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });
        builder.UseEnvironment("Development");
    }
}
public class IdentityTest:IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    public IdentityTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetEndpoint_ReturnsSuccessAndCorrectContentType()
    {
        //Arrange
        var url = "/";

        //Act
        var response =  await _client.GetAsync(url);
        var result = response.IsSuccessStatusCode;

        //Assert
        Assert.True(result);
    }
    
}