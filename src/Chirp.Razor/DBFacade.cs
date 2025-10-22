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
    
    public interface ICheepRepository
    {
        public List<CheepDto> GetCheeps(int page, int pageSize);
        public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize);
        public void CreateCheep(CheepDto cheep);

        public List<CheepDto> ReadCheeps(string authorName);

        public void UpdateCheep(CheepDto alteredCheep);

        //probably don't need getcheeps and getcheepsbyauthor here
        //thinking perhaps only read, create and update
    }

    public class ChirpRepository : ICheepRepository
    {
        private readonly ChirpDBContext _dbContext;

        public ChirpRepository(ChirpDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        //implement read, create and update methods
        //perhaps implement two methods which use the other methods to get cheeps
        
        public List<CheepDto> GetCheeps(int page, int pagesize)
        {
            var query = (from c in _dbContext.Cheeps
                         orderby c.TimeStamp descending
                         select new CheepDto
                         {
                             Author = c.Author.Name,
                             Text = c.Text,
                             UnixTimeStamp = (int)(c.TimeStamp.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
                         }).Skip((page - 1) * pagesize).Take(pagesize);
            
            return query.ToList();
        }

        public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize)
        {
            return new List<CheepDto>();
        }
        public void CreateCheep(CheepDto cheep)
        {
            //create new cheeps ? 
            //wait with persisiting to later
            

        }

        //list cheeps based on author name
        //return list of dtos
        public List<CheepDto> ReadCheeps(string authorName)
        {
            return new List<CheepDto>();
        }
        public void UpdateCheep(CheepDto alteredCheep)
        {
            //persist updates and changes to cheeps in the db ?

        }
    }
    public class ChirpDBContext : DbContext
    {
        public DbSet<Cheep> Cheeps { get; set; }
        public DbSet<Author> Authors { get; set; }

        public ChirpDBContext(DbContextOptions<ChirpDBContext> options)
            : base(options)
        {
            
        }
    }
}