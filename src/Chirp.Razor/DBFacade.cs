using Microsoft.Data.Sqlite;
using Chirp.Razor.Pages;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor
{
    public class DBFacade
    {
        private readonly string _connectionString;
        private readonly ChirpDBContext _dbContext;
        
        //Instantiates DBFacade, instantiates relvant tables in "chirp.db" if not extant
        public DBFacade(ChirpDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        //populates "chirp.db" with test data from "dump.sql"
        public void DBDataDump()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            string script = File.ReadAllText("../../data/dump.sql");
            command.CommandText = script;
            command.ExecuteNonQuery();

            connection.Close();
        }
        
        //returns a list of all cheeps in "chirp.db"
        public List<CheepViewModel> GetCheeps(int page, int pageSize)
        {
            return new List<CheepViewModel>();
        }
        
        //returns all cheeps by a specific author from "chirp.db"
        public List<CheepViewModel> GetCheepsFromAuthor(string author, int page, int pageSize)
        {
            var cheeps = new List<CheepViewModel>();

            using (var connection = new SqliteConnection(_connectionString))
            {
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
            }

            return cheeps;
        }
        private static string UnixTimeStampToDateTimeString(long unixTimeStamp)
        {
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).UtcDateTime;
            return dateTime.ToString("MM/dd/yy H:mm:ss");
        }
    }
    
}

namespace Chirp.Razor
{
    public class Author
    {
        public string Name { get; set; }
        public string Email {get; set;}
        public int AuthorId { get; set;}
        public ICollection<Cheep> Cheeps {get; set;}
    }

    public class Cheep
    {
        public int CheepId {get; set;}
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
        public int AuthorId { get; set; }
        public Author Author {get; set;}
    }

    public class CheepDto
    {
        public string Text { get; set; }
        public string Author { get; set; }
        public int UnixTimeStamp { get; set; } //how to format ???

    }
        
}