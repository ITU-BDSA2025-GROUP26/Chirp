using System;
using System.Collections.Generic;
using Chirp.Core;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

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
    }
    [Fact]
    public void OnGet_DefaultPage_FetchesPage1_Size32_SetsCheeps_AndReturnsPage()
    {
        // Arrange
        var stub = new StubCheepService();
        var expected = new List<CheepDto>
        {
            new CheepDto { Author = "alice", Text = "hi", TimeStamp = DateTime.UtcNow.ToString("O") },
            new CheepDto { Author = "bob",   Text = "yo", TimeStamp = DateTime.UtcNow.ToString("O") },
        };
        stub.NextGetCheepsResult = expected;
        var model = new PublicModel(stub);

        // Act
        var result = model.OnGet(); // default page = 1

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Same(expected, model.Cheeps);
        Assert.Equal(1, stub.LastPage);
        Assert.Equal(32, stub.LastSize);
        Assert.Null(stub.LastAuthor);
    }
}