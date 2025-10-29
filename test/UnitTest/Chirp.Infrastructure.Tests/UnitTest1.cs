using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

namespace Chirp.Infrastructure.Tests;

public class UnitTest1
{
    private ChirpDBContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())  // Using a unique in-memory database
            .Options;

        return new ChirpDBContext(options);
    }

    [Fact]
    public void GetCheeps_ReturnsPagedCheeps_Correctly()
    {
        // Arrange: Set up the in-memory database and repository
        var context = GetInMemoryDbContext();
        var author = new Author { Name = "Alice", Email = "alice@test.com" };
        context.Authors.Add(author);
        context.Cheeps.AddRange(
            new UnitTest2 { Text = "First Cheep", Author = author, TimeStamp = DateTime.Now },
            new UnitTest2 { Text = "Second Cheep", Author = author, TimeStamp = DateTime.Now }
        );
        context.SaveChanges();

        var repo = new CheepRepository(context);

        // Act: Call GetCheeps method with paging parameters (page 1, page size 1)
        var result = repo.GetCheeps(1, 1);  // Expecting one cheep (first page, page size 1)

        // Assert: Verify the result
        Assert.Single(result);  // Should return only one cheep
        Assert.Equal("First Cheep", result[0].Text);  // The text should match the first cheep
        Assert.Equal("Alice", result[0].Author);  // The author should be Alice
    }
}