using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.ComponentModel;
using System.Data.Common;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Chirp.Web.IntegrationTest;

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

    [Fact]
    public async Task Regiser_CreatesNewUser()
    {
        //Arrange
        var url ="/Account/Register";
        var formData = new Dictionary<string, string>
        {
            ["Input.Email"] = "test@mail.dk",
            ["Input.UserName"] = "usernametest",
            ["Input.Password"] = "Test123$",
            ["Input.ConfirmPassword"] = "Test123$"
        };
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();

        //Act
        var response = await _client.PostAsync(url, new FormUrlEncodedContent(formData));
        var user = dbContext.Users.FirstOrDefault(u => u.UserName == "usernametest");

        var responseString = await response.Content.ReadAsStringAsync();
        //Console.WriteLine("RESPONSE: " + responseString);

        //Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(user);
        Assert.Equal("usernametest",user.UserName);
    }
    
}