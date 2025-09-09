using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Chirp.CLI;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

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
        //System.IO.Path.Combine(
            //Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            //".chirp_cli",
            //"chirp_cli_db.csv"
        //);
        System.IO.Path.Combine(AppContext.BaseDirectory, "chirp_cli_db.csv");

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
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            UserInterface.ShowUsage();
            return 1;
        }

        switch (args[0])
        {
            case "read":
                foreach (var c in Db.Load())
                    UserInterface.ShowCheep(c);
                return 0;

            case "cheep":
                if (args.Length < 2)
                {
                    UserInterface.ShowUsage();
                    return 2;
                }
                var message = args[1];                 // quotes keep this as one arg
                var author = Environment.UserName;     // current OS user
                var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Unix timestamp
                Db.Append(new Cheep(author, message, ts));
                return 0;

            default:
                UserInterface.ShowUnknownCommand(args[0]);
                return 3;
        }
    }
}