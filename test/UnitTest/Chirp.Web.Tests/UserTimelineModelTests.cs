﻿using System;
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
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Chirp.Core.Models; 

namespace Chirp.Web.Tests;

public class UserTimelineModelTests
{
    private sealed class StubCheepService : ICheepService
    {
        public List<CheepDto>? NextGetCheepsResult { get; set; }
        public List<CheepDto>? NextGetCheepsFromAuthorResult { get; set; }
        
        public Func<string, int, int, List<CheepDto>>? GetCheepsFromAuthorHandler { get; set; }

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
            if (GetCheepsFromAuthorHandler != null)
                return GetCheepsFromAuthorHandler(author, page, pageSize);

            return NextGetCheepsFromAuthorResult ?? new List<CheepDto>();
        }

        public string? LastAddCheepAuthor { get; private set; }
        public string? LastAddCheepText { get; private set; }

        public void AddCheep(string authorUserName, string text)
        {
            LastAddCheepAuthor = authorUserName;
            LastAddCheepText = text;
        }
        
        public void LikeCheep(string authorUserName, int cheepId)
        {
            // Do something
        }
    }
    
    private sealed class StubAuthorService : IAuthorService
    {
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

        public Task<List<Author>> GetFollowers(string userNameOrEmail)
        {
            LastGetFollowersUser = userNameOrEmail;
            return Task.FromResult(NextGetFollowersResult ?? new List<Author>());
        }

        public Task<List<Author>> GetFollowing(string userNameOrEmail)
        {
            LastGetFollowingUser = userNameOrEmail;
            return Task.FromResult(NextGetFollowingResult ?? new List<Author>());
        }

        public Author GetAuthorByName(string authorName)
            => throw new NotImplementedException();

        public Author GetAuthorByEmail(string email)
            => throw new NotImplementedException();

        public void AddAuthor(string authorName, string email)
            => throw new NotImplementedException();

        public string? LastFollowFollower { get; private set; }
        public string? LastFollowFollowee { get; private set; }

        public string? LastUnfollowFollower { get; private set; }
        public string? LastUnfollowFollowee { get; private set; }

        public string? LastGetFollowersUser { get; private set; }
        public string? LastGetFollowingUser { get; private set; }

        public List<Author>? NextGetFollowersResult { get; set; }
        public List<Author>? NextGetFollowingResult { get; set; }
    }

    private static UserTimelineModel CreateModelWithUser(StubCheepService cheepStub,StubAuthorService authorStub, bool authenticated, string? userName)
    {
        var model = new UserTimelineModel(cheepStub, authorStub);

        var identity = authenticated
            ? new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Name, userName ?? string.Empty) },
                "TestAuthType")
            : new ClaimsIdentity();

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
    public void OnGet_DefaultPage_FetchesAuthorPage1_Size32_SetsCheeps_ViewData_AndReturnsPage()
    {
        // Arrange
        var author = "alice";
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var expected = new List<CheepDto>
        {
            new CheepDto { Author = author, Text = "first", TimeStamp = DateTime.UtcNow.ToString("O") }
        };
        cheepStub.NextGetCheepsFromAuthorResult = expected;
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: false, userName: null);

        // Act
        var result = model.OnGet(author);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Same(expected, model.Cheeps);
        Assert.Equal(author, cheepStub.LastAuthor);
        Assert.Equal(1, cheepStub.LastPage);
        Assert.Equal(32, cheepStub.LastSize);
        Assert.Equal(1, model.ViewData["CurrentPage"]);
        Assert.Equal(author, model.ViewData["Author"]);
    }

    [Theory]
    [InlineData("bob", 2)]
    [InlineData("charlie", 7)]
    public void OnGet_CustomPage_UsesAuthorAndPage_ReturnsEmptyIfNoData_SetsViewData(string author, int page)
    {
        // Arrange
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: false, userName: null);


        // Act
        var result = model.OnGet(author, page);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Empty(model.Cheeps);
        Assert.Equal(author, cheepStub.LastAuthor);
        Assert.Equal(page, cheepStub.LastPage);
        Assert.Equal(32, cheepStub.LastSize);
        Assert.Equal(page, model.ViewData["CurrentPage"]);
        Assert.Equal(author, model.ViewData["Author"]);
    }

    [Fact]
    public void OnGet_UsesPageNumberWhenPageIsNull()
    {
        // Arrange
        var author = "daisy";
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: false, userName: null);


        // Act
        var result = model.OnGet(author, page: null, pageNumber: 5);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Empty(model.Cheeps);
        Assert.Equal(author, cheepStub.LastAuthor);
        Assert.Equal(5, cheepStub.LastPage);
        Assert.Equal(32, cheepStub.LastSize);
        Assert.Equal(5, model.ViewData["CurrentPage"]);
        Assert.Equal(author, model.ViewData["Author"]);
    }

    [Fact]
    public void OnGet_EmptyAuthor_CallsService_WithEmptyString()
    {
        // Arrange
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: false, userName: null);


        // Act
        var result = model.OnGet(string.Empty);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Empty(model.Cheeps);
        Assert.Equal(string.Empty, cheepStub.LastAuthor);
        Assert.Equal(1, cheepStub.LastPage);
        Assert.Equal(32, cheepStub.LastSize);
        Assert.Equal(1, model.ViewData["CurrentPage"]);
        Assert.Equal(string.Empty, model.ViewData["Author"]);
    }
    
    [Fact]
    public void OnGet_ViewingOwnTimeline_LoadsOwnAndFolloweesCheeps_AndSetsFollowing()
    {
        // Arrange
        var author = "alice";

        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();

        var ownCheeps = new List<CheepDto>
        {
            new CheepDto { Author = author, Text = "own-1", TimeStamp = DateTime.UtcNow.ToString("O") }
        };
        var bobCheeps = new List<CheepDto>
        {
            new CheepDto { Author = "bob", Text = "bob-1", TimeStamp = DateTime.UtcNow.AddMinutes(-1).ToString("O") }
        };

        authorStub.NextGetFollowingResult = new List<Author>
        {
            new Author { UserName = "bob" }
        };

        cheepStub.GetCheepsFromAuthorHandler = (a, p, s) =>
        {
            if (a == author) return ownCheeps;
            if (a == "bob") return bobCheeps;
            return new List<CheepDto>();
        };

        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: author);

        // Act
        var result = model.OnGet(author, page: 1);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal(2, model.Cheeps.Count);
        Assert.Contains(model.Cheeps, c => c.Author == author && c.Text == "own-1");
        Assert.Contains(model.Cheeps, c => c.Author == "bob" && c.Text == "bob-1");
        Assert.Single(model.Following);
        Assert.Equal("bob", model.Following[0].UserName);
    }

    // -----------------------
    // OnPost tests
    // -----------------------

    [Fact]
    public void OnPost_UnauthenticatedUser_ReturnsUnauthorized_AndDoesNotCallService()
    {
        // Arrange
        var author = "alice";
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: false, userName: null);
        model.Text = "hello there";

        // Act
        var result = model.OnPost(author);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
        Assert.Null(cheepStub.LastAddCheepAuthor);
        Assert.Null(cheepStub.LastAddCheepText);
    }

    [Fact]
    public void OnPost_AuthenticatedButDifferentUser_ReturnsForbid_AndDoesNotCallService()
    {
        // Arrange
        var author = "alice";
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: "bob");
        model.Text = "hello there";

        // Act
        var result = model.OnPost(author);

        // Assert
        Assert.IsType<ForbidResult>(result);
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
        var author = "alice";
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var expected = new List<CheepDto>
        {
            new CheepDto { Author = author, Text = "existing", TimeStamp = DateTime.UtcNow.ToString("O") }
        };
        cheepStub.NextGetCheepsFromAuthorResult = expected;

        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: author);
        model.Text = text ?? string.Empty;

        // Act
        var result = model.OnPost(author, page: 3);

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.True(model.ModelState.ContainsKey(nameof(UserTimelineModel.Text)));
        Assert.Same(expected, model.Cheeps);
        Assert.Equal(author, cheepStub.LastAuthor);
        Assert.Equal(3, cheepStub.LastPage);
        Assert.Equal(32, cheepStub.LastSize);
        Assert.Equal(3, model.ViewData["CurrentPage"]);
        Assert.Equal(author, model.ViewData["Author"]);
        Assert.Null(cheepStub.LastAddCheepAuthor);
        Assert.Null(cheepStub.LastAddCheepText);
    }

    [Fact]
    public void OnPost_TextLongerThan160Characters_AddsModelError_AndReloadsCheeps()
    {
        // Arrange
        var author = "alice";
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var expected = new List<CheepDto>
        {
            new CheepDto { Author = author, Text = "existing", TimeStamp = DateTime.UtcNow.ToString("O") }
        };
        cheepStub.NextGetCheepsFromAuthorResult = expected;

        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: author);
        model.Text = new string('x', 161);

        // Act
        var result = model.OnPost(author, page: 2);

        // Assert
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.True(model.ModelState.ContainsKey(nameof(UserTimelineModel.Text)));

        Assert.Same(expected, model.Cheeps);
        Assert.Equal(author, cheepStub.LastAuthor);
        Assert.Equal(2, cheepStub.LastPage);
        Assert.Equal(32, cheepStub.LastSize);
        Assert.Equal(2, model.ViewData["CurrentPage"]);
        Assert.Equal(author, model.ViewData["Author"]);

        Assert.Null(cheepStub.LastAddCheepAuthor);
        Assert.Null(cheepStub.LastAddCheepText);
    }

    [Fact]
    public void OnPost_ValidCheep_TrimsText_CallsService_AndRedirectsToUserPageWithPageQuery()
    {
        // Arrange
        var author = "alice";
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: author);

        model.Text = "  hello world  ";

        // Act
        var result = model.OnPost(author, page: 4);

        // Assert
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/UserTimeline", redirect.PageName);
        Assert.NotNull(redirect.RouteValues);
        Assert.Equal(author, redirect.RouteValues["author"]?.ToString());
        Assert.Equal("4", redirect.RouteValues["page"]?.ToString());

        Assert.Equal(author, cheepStub.LastAddCheepAuthor);
        Assert.Equal("hello world", cheepStub.LastAddCheepText);

        // PRG pattern: OnPost does not reload cheeps when successful
        Assert.Null(cheepStub.LastPage);
        Assert.Null(cheepStub.LastSize);
    }

    [Fact]
    public void OnPost_UsesPageNumberWhenPageIsNull()
    {
        // Arrange
        var author = "alice";
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: true, userName: author);

        model.Text = "test cheep";

        // Act
        var result = model.OnPost(author, page: null, pageNumber: 7);

        // Assert
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/UserTimeline", redirect.PageName);
        Assert.NotNull(redirect.RouteValues);
        Assert.Equal(author, redirect.RouteValues["author"]?.ToString());
        Assert.Equal("7", redirect.RouteValues["page"]?.ToString());
        Assert.Equal(author, cheepStub.LastAddCheepAuthor);
        Assert.Equal("test cheep", cheepStub.LastAddCheepText);
    }
    
    [Fact]
    public void OnPostFollow_UnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        var cheepStub = new StubCheepService();
        var authorStub = new StubAuthorService();
        var model = CreateModelWithUser(cheepStub, authorStub, authenticated: false, userName: null);

        // Act
        var result = model.OnPostFollow(authorToFollow: "bob", author: "alice", page: 2);

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
        var result = model.OnPostFollow(authorToFollow: "bob", author: "alice", page: 3);

        // Assert
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/UserTimeline", redirect.PageName);
        Assert.NotNull(redirect.RouteValues);
        Assert.Equal("alice", redirect.RouteValues["author"]?.ToString());
        Assert.Equal("3", redirect.RouteValues["page"]?.ToString());
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
        var result = model.OnPostUnfollow(authorToUnfollow: "bob", author: "alice", page: 2);

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
        var result = model.OnPostUnfollow(authorToUnfollow: "bob", author: "alice", page: 4);

        // Assert
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/UserTimeline", redirect.PageName);
        Assert.NotNull(redirect.RouteValues);
        Assert.Equal("alice", redirect.RouteValues["author"]?.ToString());
        Assert.Equal("4", redirect.RouteValues["page"]?.ToString());
        Assert.Equal("alice", authorStub.LastUnfollowFollower);
        Assert.Equal("bob", authorStub.LastUnfollowFollowee);
    }
}