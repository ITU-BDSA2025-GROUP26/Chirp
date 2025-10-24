using System.Data.Common;
using Chirp.Razor.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DatabaseTest;

public class DatabaseTest
{
    private readonly DbConnection _connection;
    private readonly ChirpDBContext _context;
    public DatabaseTest()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

    }

    public void Dispose() => _connection.Dispose();
    public void CreateContext()
    {
        
    }
    [Fact]
    public void Test1()
    {

    }
}
