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
    static async Task<int> Main(string[] args)
    {
        string dbPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..","..","..", "data", "chirp.cli.db.csv");
        IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>(dbPath);
        
        var rootCommand = new RootCommand("Chirp.CLI - a simple microblogging tool");

        var readCommand = new Command("read", "Read all cheeps");
        readCommand.SetHandler(() =>
        {
            foreach (var c in database.Read())
                UserInterface.ShowCheep(c);
        });
        rootCommand.AddCommand(readCommand);

        var messageArg = new Argument<string>("message", "Message to post");
        var cheepCommand = new Command("cheep", "Post a new cheep");
        cheepCommand.AddArgument(messageArg);

        cheepCommand.SetHandler((string message) =>
        {
            var author = Environment.UserName;
            var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            database.Store(new Cheep(author, message, ts));
        }, messageArg);

        rootCommand.AddCommand(cheepCommand);

        return await rootCommand.InvokeAsync(args);
    }
}