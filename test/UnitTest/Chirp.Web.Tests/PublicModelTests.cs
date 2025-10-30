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
}