using Xunit;
using Microsoft.Data.Sqlite;
using Chirp.Infrastructure;
using Chirp.Core.Models;
using Chirp.Infrastructure.Chirp.Repositories;
using Microsoft.EntityFrameworkCore;
namespace Chirp.InfrastructureTests;

public class DatabaseTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ChirpDBContext> _contextOptions;
    public DatabaseTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        _contextOptions = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new ChirpDBContext(_contextOptions);

        context.Database.EnsureCreated();

        context.AddRange(
            new Author { Name = "Bob", Email = "bob@mail.dk", AuthorId = 1 },
            new Author { Name = "Ib", Email = "ib@mail.dk", AuthorId = 2 },
            new Cheep { Text = "Hello", TimeStamp = DateTime.Now, CheepId = 1, AuthorId = 1 },
            new Cheep { Text = "Halloween", TimeStamp = DateTime.Now, CheepId = 2, AuthorId = 2 }
        );
        context.SaveChanges();
    }
    ChirpDBContext CreateContext() => new ChirpDBContext(_contextOptions);
    public void Dispose() => _connection.Dispose();

    [Fact]
    public void CheepRepository_Uses_SQLiteInMemooryDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var cheepRepository = new CheepRepository(context);

        // Act
        var list = cheepRepository.GetCheeps(1, 10);

        // Assert
        Assert.Contains(list, c => c.Text == "Hello" && c.Author == "Bob");
        Assert.Contains(list, c => c.Text == "Halloween" && c.Author == "Ib");
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void CheepRepository_Can_GetAuthorByName()
    {
        // Arrange
        using var context = CreateContext();
        var cheepRepository = new CheepRepository(context);

        // Act
        var author = cheepRepository.GetAuthorByName("Bob");

        // Assert
        Assert.Equal("Bob", author.Name);
    }

    [Fact]
    public void CheepRepository_Can_GetAuthorByEmail()
    {
        // Arrange
        using var context = CreateContext();
        var cheepRepository = new CheepRepository(context);

        // Act
        var author = cheepRepository.GetAuthorByEmail("bob@mail.dk");

        // Assert
        Assert.Equal("Bob", author.Name);
    }
}
