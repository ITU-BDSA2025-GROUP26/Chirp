using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Chirp.Razor
{
    public class DBFacade
    {
        private readonly string _connectionString;

        //Instantiates DBFacade, instantiates relvant tables in "chirp.db" if not extant
        public DBFacade()
        {
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
        public List<CheepViewModel> GetCheeps()
        {
            var cheeps = new List<CheepViewModel>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT u.username, m.text, m.pub_date
                FROM message m
                JOIN user u ON m.author_id = u.user_id
                ORDER BY m.pub_date DESC";

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
        public List<CheepViewModel> GetCheepsFromAuthor(string author)
        {
            var cheeps = new List<CheepViewModel>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT u.username, m.text, m.pub_date
                    FROM message m
                    JOIN user u ON m.author_id = u.user_id
                    WHERE u.username = $author
                    ORDER BY m.pub_date DESC";
                command.Parameters.AddWithValue("@author", author);

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