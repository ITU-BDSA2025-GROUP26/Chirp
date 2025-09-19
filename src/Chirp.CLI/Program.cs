using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Chirp.CLI;
using System.CommandLine;
using SimpleDB;

public record Cheep(string Author, string Message, long Timestamp);

public static class Formatting
{
    public static string Pretty(Cheep c)
    {
        // Convert Unix ts to local time and match the example-ish format
        var dt = DateTimeOffset.FromUnixTimeSeconds(c.Timestamp).ToLocalTime().DateTime;
        var stamp = dt.ToString("dd/MM/yy HH:mm:ss", CultureInfo.InvariantCulture);
        return $"{c.Author} @ {stamp}: {c.Message}";
    }
}

class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        IDatabaseRepository<Cheep> database = CSVDatabase<Cheep>.Instance;

        app.MapGet("/cheeps", () => { return database.Read(); });

        app.MapPost("/cheep", (Cheep cheep) =>
        {
            database.Store(cheep);
            return Results.Ok(new { Status = "Stored", Cheep = cheep });
        });

        app.Run();
    }
}
