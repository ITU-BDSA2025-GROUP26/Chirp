using Chirp.Infrastructure.Chirp.Service;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure;
using Chirp.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Chirp.Core.Models;
using Chirp.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<Author>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ChirpDBContext>();
builder.Services
    .AddAuthentication(options =>
    {
        options.RequireAuthenticatedSignIn = true;
    })
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["authentication_github_clientId"]!;
        options.ClientSecret = builder.Configuration["authentication_github_clientSecret"]!;
        options.CallbackPath = "/signin-github";
    });
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(60);
});
var app = builder.Build();

// Create a disposable service scope
using (var scope = app.Services.CreateScope())
{
    // From the scope, get an instance of our database context.
    // Through the `using` keyword, we make sure to dispose it after we are done.
    using var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();

    // Execute the migration from code.
    context.Database.Migrate();
    DbInitializer.SeedDatabase(context);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

public partial class Program { } // For integration tests