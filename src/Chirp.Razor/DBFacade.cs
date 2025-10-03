using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Chirp.Razor.Pages;

namespace Chirp.Razor
{
    public class DBFacade
    {
        private readonly string _connectionString;
        //private int page;
        //private int pagesize;

        //Instantiates DBFacade, instantiates relvant tables in "chirp.db" if not extant
        public DBFacade()
        {
            //page = 32;
            //pagesize = 12;
            var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH")
                         ?? Path.Combine(Path.GetTempPath(), "chirp.db");

            _connectionString = $"Data Source={dbPath}";

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            string script = File.ReadAllText("../../data/schema.sql");
            command.CommandText = script;
            command.ExecuteNonQuery();

            connection.Close();
            DBDataDump();
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
            var cheeps = new List<CheepViewModel>();
            using var connection = new SqliteConnection(_connectionString);
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
    
    class Author
    {
        string Name { get; set; }
        string Email {get; set;}
    
        ICollection<Cheep> Cheeps {get; set;}
    }

    class Cheep
    {
        string Text { get; set; } 
        Datetime Timestamp { get; set; }
        Author Author { get; set; }
    }

    class CheepDto
    {
    
    }
}

namespace Chirp.Razor
{
    public interface ICheepRepository
    {
        public void CreateCheep(CheepDto cheep)
        {

        }

        public List<CheepDto> ReadCheeps(string authorName)
        {

        }

        public void UpdateCheep(CheepDto alteredCheep)
        {



        }
    }

    public class CheepDBContext : DbContext
    {
        DbSet<Cheep> cheeps { get; set; }
        DbSet<Author> authors { get; set; }

        public CheepDBContext(DbContextOptions<CheepDBContext> options)
            : base(options)
        {
        }
    }
}