using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Chirp.CLI;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.ComponentModel;
using System.Runtime.Intrinsics.Arm;

public record Cheep(string Author, string Message, long Timestamp);

public class Messages
{
    //[Name("Author")]
    [Index(0)]
    public required string Author  { get; set; }
    //[Name("Message")]
    [Index(1)]
    public required string Message { get; set; }
    //[Name("Timestamp")]
    [Index(2)]
    public long Timestamp { get; set; }
}
    

static class Db
{
    // Store DB next to the executable
    public static readonly string PathToCsv =
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "data", "chirp.cli.db.csv");


    public static IEnumerable<Cheep> Load()
    {
        if (!File.Exists(PathToCsv)) yield break;
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };
        
        using var reader = new StreamReader(PathToCsv);
        using var csv = new CsvReader(reader, config);
        
        var records = csv.GetRecords<Messages>();

        foreach (var r in records)
        {
            yield return new Cheep(r.Author, r.Message, r.Timestamp);
        }
        
    }

    public static void Append(Cheep c)
    {
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(PathToCsv)!);
        
        var fileExists = File.Exists(PathToCsv);
        
        using var stream = new FileStream(PathToCsv, FileMode.Append, FileAccess.Write, FileShare.Read);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        if (!fileExists)
        {
            csv.WriteHeader<Messages>();
            csv.NextRecord();
        }
        
        csv.WriteRecord(new Messages { Author = c.Author, Message = c.Message, Timestamp = c.Timestamp });
        csv.NextRecord();
    }
}

static class Formatting
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
        var rootCommand = new RootCommand("Chirp.CLI - a simple microblogging tool");

        var readCommand = new Command("read", "Read all cheeps");
        readCommand.SetHandler(() =>
        {
            foreach (var c in Db.Load())
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
            Db.Append(new Cheep(author, message, ts));
        }, messageArg);

        rootCommand.AddCommand(cheepCommand);

        return await rootCommand.InvokeAsync(args);
    }
}