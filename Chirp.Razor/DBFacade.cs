using Microsoft.Data.Sqlite;

namespace Chirp.Razor.Pages;

public class DBFacade
{
    private readonly string _path;

    public DBFacade(string dbPath)
    {
        _path = dbPath;
    }

    public List<CheepViewModel> GetCheeps(int page, int pageSize)
    {
        var cheeps = new List<CheepViewModel>();
        using var connection = new SqliteConnection($"Data Source={_path}");
        connection.Open();

        var offset = (page - 1) * pageSize;

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT u.username, m.text, m.pub_date
            FROM message m
            JOIN user u ON m.author_id = u.user_id
            ORDER BY m.pub_date DESC
            LIMIT $pageSize OFFSET $offset";
        command.Parameters.AddWithValue("$pageSize", pageSize);
        command.Parameters.AddWithValue("$offset", offset);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            cheeps.Add(new CheepViewModel(
                reader.GetString(0),
                reader.GetString(1),
                UnixTimeStampToDateTimeString(reader.GetInt64(2))
            ));
        }
        return cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page, int pageSize)
    {
        var cheeps = new List<CheepViewModel>();
        using var connection = new SqliteConnection($"Data Source={_path}");
        connection.Open();

        var offset = (page - 1) * pageSize;

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT u.username, m.text, m.pub_date
            FROM message m
            JOIN user u ON m.author_id = u.user_id
            WHERE u.username = $author
            ORDER BY m.pub_date DESC
            LIMIT $pageSize OFFSET $offset";
        command.Parameters.AddWithValue("$author", author);
        command.Parameters.AddWithValue("$pageSize", pageSize);
        command.Parameters.AddWithValue("$offset", offset);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            cheeps.Add(new CheepViewModel(
                reader.GetString(0),
                reader.GetString(1),
                UnixTimeStampToDateTimeString(reader.GetInt64(2))
            ));
        }
        return cheeps;
    }

    private static string UnixTimeStampToDateTimeString(long unixTimeStamp)
    {
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).UtcDateTime;
        return dateTime.ToString("dd/MM/yy H:mm:ss");
    }
}
