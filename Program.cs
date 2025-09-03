using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

public record Cheep(string Author, string Message, long Timestamp);

public class Messages
{
    public string Author  { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }
}
    

static class Db
{
    // Store DB next to the executable
    public static readonly string PathToCsv =
        System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".chirp_cli",
            "chirp_cli_db.csv"
        );
        //System.IO.Path.Combine(AppContext.BaseDirectory, "chirp_cli_db.csv");

    public static IEnumerable<Cheep> Load()
    {
        if (!File.Exists(PathToCsv)) yield break;
        
        using var reader = new StreamReader(PathToCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<Messages>();

        foreach (var r in records)
        {
            yield return new Cheep(r.Author, r.Message, r.Timestamp);
        }

        /*foreach (var line in File.ReadLines(PathToCsv))
        {
            // CSV: author,unix_ts,message  (message may contain commas, so we allow quotes)
            // Very light parser: split first 2 commas; strip wrapping quotes on message.
            var firstComma = line.IndexOf(',');
            if (firstComma < 0) continue;
            var secondComma = line.IndexOf(',', firstComma + 1);
            if (secondComma < 0) continue;

            var author = line[..firstComma];
            var tsText = line[(firstComma + 1)..secondComma];
            var msg = line[(secondComma + 1)..];
            if (msg.Length >= 2 && msg.StartsWith('"') && msg.EndsWith('"'))
                msg = msg[1..^1].Replace("\"\"", "\""); // unescape double quotes

            if (!long.TryParse(tsText, out var ts)) continue;

            yield return new Cheep(author, ts, msg);
        }
        */
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
        // Escape message for CSV: wrap in quotes and double any quotes inside
        //var safeMsg = "\"" + c.Message.Replace("\"", "\"\"") + "\"";
        //File.AppendAllText(PathToCsv, $"{c.Author},{c.Timestamp},{safeMsg}{Environment.NewLine}");
    }
}

static class Formatting
{
    public static string Pretty(Cheep c)
    {
        // Convert Unix ts to local time and match the example-ish format
        var dt = DateTimeOffset.FromUnixTimeSeconds(c.Timestamp).ToLocalTime().DateTime;
        var stamp = dt.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture); // adjust if teacher wants dd/MM/yy
        return $"{c.Author} @ {stamp}: {c.Timestamp}";
    }
}

class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: read | cheep \"message\"");
            return 1;
        }

        switch (args[0])
        {
            case "read":
                foreach (var c in Db.Load())
                    Console.WriteLine(Formatting.Pretty(c));
                return 0;

            case "cheep":
                if (args.Length < 2)
                {
                    Console.Error.WriteLine("cheep requires a message, e.g. cheep \"Hello, world!\"");
                    return 2;
                }
                var message = args[1];                 // quotes keep this as one arg
                var author = Environment.UserName;     // current OS user
                var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Unix timestamp
                Db.Append(new Cheep(author, message, ts));
                return 0;

            default:
                Console.Error.WriteLine($"Unknown command: {args[0]}");
                return 3;
        }
    }
}