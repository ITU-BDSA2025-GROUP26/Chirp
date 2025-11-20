using System;
using System.Collections.Generic;
using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Xunit;

namespace Chirp.Web.Tests;

public class UserTimelineModelTests
{
    private sealed class StubCheepService : ICheepService
    {
        public List<CheepDto>? NextGetCheepsResult { get; set; }
        public List<CheepDto>? NextGetCheepsFromAuthorResult { get; set; }

        public int? LastPage { get; private set; }
        public int? LastSize { get; private set; }
        public string? LastAuthor { get; private set; }

        public List<CheepDto> GetCheeps(int page, int pageSize)
        {
            LastAuthor = null;
            LastPage = page;
            LastSize = pageSize;
            return NextGetCheepsResult ?? new List<CheepDto>();
        }

        public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize)
        {
            LastAuthor = author;
            LastPage = page;
            LastSize = pageSize;
            return NextGetCheepsFromAuthorResult ?? new List<CheepDto>();
        }

        public string? LastAddCheepAuthor { get; private set; }
        public string? LastAddCheepText { get; private set; }

        public void AddCheep(string authorUserName, string text)
        {
            LastAddCheepAuthor = authorUserName;
            LastAddCheepText = text;
        }
    }

    private static UserTimelineModel CreateModelWithUser(StubCheepService stub, bool authenticated, string? userName)
    {
        var model = new UserTimelineModel(stub);

        var identity = authenticated
            ? new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Name, userName ?? string.Empty) },
                "TestAuthType")
            : new ClaimsIdentity(); // unauthenticated

        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        model.PageContext = new PageContext
        {
            HttpContext = httpContext
        };

        return model;
    }

    // -----------------------
    // OnGet tests
    // -----------------------

    [Fact]
    public void OnGet_DefaultPage_FetchesAuthorPage1_Size32_SetsCheeps_ViewData_AndReturnsPage()
    {
        // Arrange
        var author = "alice";
        var stub = new StubCheepService();
        var expected = new List<CheepDto>
        {
            new CheepDto { Author = author, Text = "first", TimeStamp = DateTime.UtcNow.ToString("O") }
        };
        stub.NextGetCheepsFromAuthorResult = expected;
        var model = new UserTimelineModel(stub);

        // Act
        var result = model.OnGet(author);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Same(expected, model.Cheeps);
        Assert.Equal(author, stub.LastAuthor);
        Assert.Equal(1, stub.LastPage);
        Assert.Equal(32, stub.LastSize);
        Assert.Equal(1, model.ViewData["CurrentPage"]);
        Assert.Equal(author, model.ViewData["Author"]);
    }

    [Theory]
    [InlineData("bob", 2)]
    [InlineData("charlie", 7)]
    public void OnGet_CustomPage_UsesAuthorAndPage_ReturnsEmptyIfNoData_SetsViewData(string author, int page)
    {
        // Arrange
        var stub = new StubCheepService();
        var model = new UserTimelineModel(stub);

        // Act
        var result = model.OnGet(author, page);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Empty(model.Cheeps);
        Assert.Equal(author, stub.LastAuthor);
        Assert.Equal(page, stub.LastPage);
        Assert.Equal(32, stub.LastSize);
        Assert.Equal(page, model.ViewData["CurrentPage"]);
        Assert.Equal(author, model.ViewData["Author"]);
    }

    [Fact]
    public void OnGet_UsesPageNumberWhenPageIsNull()
    {
        // Arrange
        var author = "daisy";
        var stub = new StubCheepService();
        var model = new UserTimelineModel(stub);

        // Act
        var result = model.OnGet(author, page: null, pageNumber: 5);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Empty(model.Cheeps);
        Assert.Equal(author, stub.LastAuthor);
        Assert.Equal(5, stub.LastPage);
        Assert.Equal(32, stub.LastSize);
        Assert.Equal(5, model.ViewData["CurrentPage"]);
        Assert.Equal(author, model.ViewData["Author"]);
    }

    [Fact]
    public void OnGet_EmptyAuthor_CallsService_WithEmptyString()
    {
        // Arrange
        var stub = new StubCheepService();
        var model = new UserTimelineModel(stub);

        // Act
        var result = model.OnGet(string.Empty);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Empty(model.Cheeps);
        Assert.Equal(string.Empty, stub.LastAuthor);
        Assert.Equal(1, stub.LastPage);
        Assert.Equal(32, stub.LastSize);
        Assert.Equal(1, model.ViewData["CurrentPage"]);
        Assert.Equal(string.Empty, model.ViewData["Author"]);
    }

    // -----------------------
    // OnPost tests
    // -----------------------

    [Fact]
    public void OnPost_UnauthenticatedUser_ReturnsUnauthorized_AndDoesNotCallService()
    {
        // Arrange
        var author = "alice";
        var stub = new StubCheepService();
        var model = CreateModelWithUser(stub, authenticated: false, userName: null);
        model.Text = "hello there";

        // Act
        var result = model.OnPost(author);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
        Assert.Null(stub.LastAddCheepAuthor);
        Assert.Null(stub.LastAddCheepText);
    }

    [Fact]
    public void OnPost_AuthenticatedButDifferentUser_ReturnsForbid_AndDoesNotCallService()
    {
        // Arrange
        var author = "alice";
        var stub = new StubCheepService();
        var model = CreateModelWithUser(stub, authenticated: true, userName: "bob");
        model.Text = "hello there";

        // Act
        var result = model.OnPost(author);

        // Assert
        Assert.IsType<ForbidResult>(result);
        Assert.Null(stub.LastAddCheepAuthor);
        Assert.Null(stub.LastAddCheepText);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void OnPost_Invalid_EmptyOrWhitespaceText_ReRendersPage_WithModelError_AndReloadsCheeps(string? text)
    {
        // Arrange
        var author = "alice";
        var stub = new StubCheepService();
        var expected = new List<CheepDto>
        {
            new CheepDto { Author = author, Text = "existing", TimeStamp = DateTime.UtcNow.ToString("O") }
        };
        stub.NextGetCheepsFromAuthorResult = expected;

        var model = CreateModelWithUser(stub, authenticated: true, userName: author);
        model.Text = text ?? string.Empty;

        // Act
        var result = model.OnPost(author, page: 3);

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.True(model.ModelState.ContainsKey(nameof(UserTimelineModel.Text)));

        Assert.Same(expected, model.Cheeps);
        Assert.Equal(author, stub.LastAuthor);
        Assert.Equal(3, stub.LastPage);
        Assert.Equal(32, stub.LastSize);
        Assert.Equal(3, model.ViewData["CurrentPage"]);
        Assert.Equal(author, model.ViewData["Author"]);

        Assert.Null(stub.LastAddCheepAuthor);
        Assert.Null(stub.LastAddCheepText);
    }

    [Fact]
    public void OnPost_TextLongerThan160Characters_AddsModelError_AndReloadsCheeps()
    {
        // Arrange
        var author = "alice";
        var stub = new StubCheepService();
        var expected = new List<CheepDto>
        {
            new CheepDto { Author = author, Text = "existing", TimeStamp = DateTime.UtcNow.ToString("O") }
        };
        stub.NextGetCheepsFromAuthorResult = expected;

        var model = CreateModelWithUser(stub, authenticated: true, userName: author);
        model.Text = new string('x', 161);

        // Act
        var result = model.OnPost(author, page: 2);

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.True(model.ModelState.ContainsKey(nameof(UserTimelineModel.Text)));

        Assert.Same(expected, model.Cheeps);
        Assert.Equal(author, stub.LastAuthor);
        Assert.Equal(2, stub.LastPage);
        Assert.Equal(32, stub.LastSize);
        Assert.Equal(2, model.ViewData["CurrentPage"]);
        Assert.Equal(author, model.ViewData["Author"]);

        Assert.Null(stub.LastAddCheepAuthor);
        Assert.Null(stub.LastAddCheepText);
    }

    [Fact]
    public void OnPost_ValidCheep_TrimsText_CallsService_AndRedirectsToUserPageWithPageQuery()
    {
        // Arrange
        var author = "alice";
        var stub = new StubCheepService();
        var model = CreateModelWithUser(stub, authenticated: true, userName: author);

        model.Text = "  hello world  ";

        // Act
        var result = model.OnPost(author, page: 4);

        // Assert
        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal($"/{author}?page=4", redirect.Url);

        Assert.Equal(author, stub.LastAddCheepAuthor);
        Assert.Equal("hello world", stub.LastAddCheepText); // trimmed

        // PRG pattern: OnPost does not reload cheeps when successful
        Assert.Null(stub.LastPage);
        Assert.Null(stub.LastSize);
    }

    [Fact]
    public void OnPost_UsesPageNumberWhenPageIsNull()
    {
        // Arrange
        var author = "alice";
        var stub = new StubCheepService();
        var model = CreateModelWithUser(stub, authenticated: true, userName: author);

        model.Text = "test cheep";

        // Act
        var result = model.OnPost(author, page: null, pageNumber: 7);

        // Assert
        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal($"/{author}?page=7", redirect.Url);
        Assert.Equal(author, stub.LastAddCheepAuthor);
        Assert.Equal("test cheep", stub.LastAddCheepText);
    }
}