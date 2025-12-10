using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

namespace Chirp.Infrastructure.Tests;

public class CheepsPageTest
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
        // Arrange
        var context = GetInMemoryDbContext();
        var author = new Author { UserName = "Daid", Email = "daid@itu.com" };
        context.Authors.Add(author);
        context.Cheeps.AddRange(
            new Cheep { Text = "First Cheep", Author = author, TimeStamp = DateTime.Now },
            new Cheep { Text = "Second Cheep", Author = author, TimeStamp = DateTime.Now }
        );
        context.SaveChanges();

        var repo = new CheepRepository(context);

        // Act
        var result = repo.GetCheeps(1, 1); 

        // Assert
        Assert.Single(result);
        Assert.Equal("Second Cheep", result[0].Text);
        Assert.Equal("Daid", result[0].Author); 
    }
}