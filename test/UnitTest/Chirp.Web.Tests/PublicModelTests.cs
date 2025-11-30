using System;
using System.Collections.Generic;
using System.Security.Claims;
using Chirp.Core;
using Chirp.Core.Models;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace Chirp.Web.Tests;

public class PublicModelTests
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
    private sealed class StubAuthorService : IAuthorService
    {
        public Author GetAuthorByName(string authorName) => new Author();
        public Author GetAuthorByEmail(string email) => new Author();
        public void AddAuthor(string authorName, string email) { }

        public Task Follow(string followerUserName, string followeeUserName)
        {
            LastFollowFollower = followerUserName;
            LastFollowFollowee = followeeUserName;
            return Task.CompletedTask;
        }

        public Task Unfollow(string followerUserName, string followeeUserName)
        {
            LastUnfollowFollower = followerUserName;
            LastUnfollowFollowee = followeeUserName;
            return Task.CompletedTask;
        }

        public Task<List<Author>> GetFollowing(string userNameOrEmail)
        {
            LastGetFollowingUser = userNameOrEmail;
            return Task.FromResult(NextGetFollowingResult ?? new List<Author>());
        }

        public Task<List<Author>> GetFollowers(string userNameOrEmail)
        {
            LastGetFollowersUser = userNameOrEmail;
            return Task.FromResult(NextGetFollowersResult ?? new List<Author>());
        }

        public string? LastFollowFollower { get; private set; }
        public string? LastFollowFollowee { get; private set; }
        public string? LastUnfollowFollower { get; private set; }
        public string? LastUnfollowFollowee { get; private set; }
        public string? LastGetFollowingUser { get; private set; }
        public string? LastGetFollowersUser { get; private set; }

        public List<Author>? NextGetFollowingResult { get; set; }
        public List<Author>? NextGetFollowersResult { get; set; }
    }
    
    private static PublicModel CreateModelWithUser(StubCheepService stub, StubAuthorService authorStub, bool authenticated,
        string? userName = "alice")
    {
        var model = new PublicModel(stub,authorStub);

        var identity = new ClaimsIdentity();
        if (authenticated)
        {
            identity = new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.Name, userName ?? "alice"),
                },
                "TestAuthType");
        }


        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext()
        {
            User = principal
        };
        
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        
        var modelState = new ModelStateDictionary();

        var viewData = new ViewDataDictionary(
            new EmptyModelMetadataProvider(),
            modelState
        );

        var pageContext = new PageContext(actionContext)
        {
            ViewData = viewData
        };

        model.PageContext = pageContext;
        
        return model;
    }
    
    // -----------------------
    // OnGet tests
    // -----------------------


    [Fact]
    public void OnGet_DefaultPage_FetchesPage1_Size32_SetsCheeps_AndReturnsPage()
    {
        // Arrange
        var stub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var expected = new List<CheepDto>
        {
            new CheepDto { Author = "alice", Text = "hi", TimeStamp = DateTime.UtcNow.ToString("O") },
            new CheepDto { Author = "bob",   Text = "yo", TimeStamp = DateTime.UtcNow.ToString("O") },
        };
        stub.NextGetCheepsResult = expected;
        var model = CreateModelWithUser(stub, authorStub, false, userName: null);

        

        // Act
        var result = model.OnGet();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Same(expected, model.Cheeps);
        Assert.Equal(1, stub.LastPage);
        Assert.Equal(32, stub.LastSize);
        Assert.Null(stub.LastAuthor);
        Assert.Equal(1, model.ViewData["CurrentPage"]);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(9)]
    public void OnGet_CustomPage_UsesProvidedPage_AndReturnsEmptyIfNoData(int page)
    {
        // Arrange
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: false, userName: null);

        

        // Act
        var result = model.OnGet(page);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Empty(model.Cheeps);
        Assert.Equal(page, cheepStub.LastPage);
        Assert.Equal(32, cheepStub.LastSize);
        Assert.Equal(page, model.ViewData["CurrentPage"]);
    }
    
    [Fact]
    public void OnGet_PageNumber_Fallback_IsUsed_WhenPageIsNull()
    {
        // Arrange
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: false, userName: null);


        // Act
        var result = model.OnGet(page: null, pageNumber: 5);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal(5, cheepStub.LastPage);
        Assert.Equal(32, cheepStub.LastSize);
        Assert.Equal(5, model.ViewData["CurrentPage"]);
    }
    
    [Fact]
    public void OnGet_AuthenticatedUser_LoadsFollowing()
    {
        // Arrange
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var following = new List<Author>
        {
            new Author { UserName = "bob" },
            new Author { UserName = "charlie" }
        };
        authorStub.NextGetFollowingResult = following;

        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: "alice");

        // Act
        var result = model.OnGet();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal(2, model.Following.Count);
        Assert.Contains(model.Following, a => a.UserName == "bob");
        Assert.Contains(model.Following, a => a.UserName == "charlie");
    }

    // -----------------------
    // OnPost tests
    // -----------------------
    
    [Fact]
    public void OnPost_UnauthenticatedUser_ReturnsUnauthorized_AndDoesNotCallService()
    {
        // Arrange
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: false);
        model.Text = "hello world";

        // Act
        var result = model.OnPost();

        // Assert
        var unauthorized = Assert.IsType<UnauthorizedResult>(result);
        Assert.Null(cheepStub.LastAddCheepAuthor);
        Assert.Null(cheepStub.LastAddCheepText);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void OnPost_Invalid_EmptyOrWhitespaceText_ReRendersPage_WithModelError_AndReloadsCheeps(string? text)
    {
        // Arrange
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var expected = new List<CheepDto>
        {
            new CheepDto { Author = "bob", Text = "existing", TimeStamp = DateTime.UtcNow.ToString("O") }
        };
        cheepStub.NextGetCheepsResult = expected;

        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: "alice");
        model.Text = text ?? string.Empty;

        // Act
        var result = model.OnPost(page: 3);

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.True(model.ModelState.ContainsKey(nameof(PublicModel.Text)));
        Assert.Same(expected, model.Cheeps);
        Assert.Equal(3, cheepStub.LastPage);
        Assert.Equal(32, cheepStub.LastSize);
        Assert.Equal(3, model.ViewData["CurrentPage"]);
        Assert.Null(cheepStub.LastAddCheepAuthor);
        Assert.Null(cheepStub.LastAddCheepText);
    }

    [Fact]
    public void OnPost_TextLongerThan160Characters_AddsModelError_AndReloadsCheeps()
    {
        // Arrange
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var expected = new List<CheepDto>
        {
            new CheepDto { Author = "bob", Text = "existing", TimeStamp = DateTime.UtcNow.ToString("O") }
        };
        cheepStub.NextGetCheepsResult = expected;

        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: "alice");
        model.Text = new string('x', 161); // over limit

        // Act
        var result = model.OnPost(page: 2);

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.True(model.ModelState.ContainsKey(nameof(PublicModel.Text)));
        Assert.Same(expected, model.Cheeps);
        Assert.Equal(2, cheepStub.LastPage);
        Assert.Equal(32, cheepStub.LastSize);
        Assert.Equal(2, model.ViewData["CurrentPage"]);
        Assert.Null(cheepStub.LastAddCheepAuthor);
        Assert.Null(cheepStub.LastAddCheepText);
    }

    [Fact]
    public void OnPost_ValidCheep_TrimsText_CallsService_AndRedirectsToSamePage()
    {
        // Arrange
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: "alice");

        model.Text = "  hello world  ";

        // Act
        var result = model.OnPost(page: 4);

        // Assert
        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("?page=4", redirect.Url);

        Assert.Equal("alice", cheepStub.LastAddCheepAuthor);
        Assert.Equal("hello world", cheepStub.LastAddCheepText); // trimmed

        // On successful post, GetCheeps is not called in OnPost (PRG pattern),
        // so LastPage/LastSize should still be null.
        Assert.Null(cheepStub.LastPage);
        Assert.Null(cheepStub.LastSize);
    }

    [Fact]
    public void OnPost_UsesPageNumberWhenPageIsNull()
    {
        // Arrange
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: "alice");

        model.Text = "test cheep";

        // Act
        var result = model.OnPost(page: null, pageNumber: 7);

        // Assert
        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("?page=7", redirect.Url);
        Assert.Equal("alice", cheepStub.LastAddCheepAuthor);
        Assert.Equal("test cheep", cheepStub.LastAddCheepText);
    }
    
    [Fact]
    public void OnPost_InvalidText_WhenAuthenticated_ReloadsCheeps_AndFollowing()
    {
        // Arrange
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();

        var expectedCheeps = new List<CheepDto>
        {
            new CheepDto { Author = "bob", Text = "existing", TimeStamp = DateTime.UtcNow.ToString("O") }
        };
        cheepStub.NextGetCheepsResult = expectedCheeps;

        var following = new List<Author>
        {
            new Author { UserName = "bob" }
        };
        authorStub.NextGetFollowingResult = following;

        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: "alice");
        model.Text = ""; // invalid

        // Act
        var result = model.OnPost(page: 3);

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);

        Assert.Same(expectedCheeps, model.Cheeps);
        Assert.Single(model.Following);
        Assert.Equal("bob", model.Following[0].UserName);
    }
    
    [Fact]
public void OnPostFollow_UnauthenticatedUser_ReturnsUnauthorized()
{
    // Arrange
    var cheepStub = new StubCheepService();
    var authorStub = new StubAuthorService();
    var model = CreateModelWithUser(cheepStub, authorStub, authenticated: false, userName: null);

    // Act
    var result = model.OnPostFollow(authorToFollow: "bob", page: 2);

    // Assert
    Assert.IsType<UnauthorizedResult>(result);
    Assert.Null(authorStub.LastFollowFollower);
    Assert.Null(authorStub.LastFollowFollowee);
}

[Fact]
public void OnPostFollow_AuthenticatedUser_CallsAuthorServiceAndRedirects()
{
    // Arrange
    var cheepStub = new StubCheepService();
    var authorStub = new StubAuthorService();
    var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: "alice");

    // Act
    var result = model.OnPostFollow(authorToFollow: "bob", page: 3);

    // Assert
    var redirect = Assert.IsType<RedirectResult>(result);
    Assert.Equal("?page=3", redirect.Url);

    Assert.Equal("alice", authorStub.LastFollowFollower);
    Assert.Equal("bob", authorStub.LastFollowFollowee);
}

[Fact]
public void OnPostUnfollow_UnauthenticatedUser_ReturnsUnauthorized()
{
    // Arrange
    var cheepStub = new StubCheepService();
    var authorStub = new StubAuthorService();
    var model = CreateModelWithUser(cheepStub, authorStub, authenticated: false, userName: null);

    // Act
    var result = model.OnPostUnfollow(authorToUnfollow: "bob", page: 2);

    // Assert
    Assert.IsType<UnauthorizedResult>(result);
    Assert.Null(authorStub.LastUnfollowFollower);
    Assert.Null(authorStub.LastUnfollowFollowee);
}

[Fact]
public void OnPostUnfollow_AuthenticatedUser_CallsAuthorServiceAndRedirects()
{
    // Arrange
    var cheepStub = new StubCheepService();
    var authorStub = new StubAuthorService();
    var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: "alice");

    // Act
    var result = model.OnPostUnfollow(authorToUnfollow: "bob", page: 4);

    // Assert
    var redirect = Assert.IsType<RedirectResult>(result);
    Assert.Equal("?page=4", redirect.Url);

    Assert.Equal("alice", authorStub.LastUnfollowFollower);
    Assert.Equal("bob", authorStub.LastUnfollowFollowee);
}
}