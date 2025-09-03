using System;
using System.Globalization;
using System.IO;
using System.Linq;

record Cheep(string Author, long UnixTs, string Message);

static class Db
{
    // Store DB next to the executable
    public static readonly string PathToCsv =
        System.IO.Path.Combine(AppContext.BaseDirectory, "chirp_cli_db.csv");

    public static IEnumerable<Cheep> Load()
    {
        if (!File.Exists(PathToCsv)) yield break;

        foreach (var line in File.ReadLines(PathToCsv))
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
    }

    public static void Append(Cheep c)
    {
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(PathToCsv)!);
        // Escape message for CSV: wrap in quotes and double any quotes inside
        var safeMsg = "\"" + c.Message.Replace("\"", "\"\"") + "\"";
        File.AppendAllText(PathToCsv, $"{c.Author},{c.UnixTs},{safeMsg}{Environment.NewLine}");
    }
}

static class Formatting
{
    public static string Pretty(Cheep c)
    {
        // Convert Unix ts to local time and match the example-ish format
        var dt = DateTimeOffset.FromUnixTimeSeconds(c.UnixTs).ToLocalTime().DateTime;
        var stamp = dt.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture); // adjust if teacher wants dd/MM/yy
        return $"{c.Author} @ {stamp}: {c.Message}";
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
                Db.Append(new Cheep(author, ts, message));
                return 0;

            default:
                Console.Error.WriteLine($"Unknown command: {args[0]}");
                return 3;
        }
    }
}