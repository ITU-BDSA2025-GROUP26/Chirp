using Chirp.Razor;

namespace  Chirp.Razor
{

    public record CheepViewModel(string Author, string Message, string Timestamp);

    public interface ICheepService
    {
        public List<CheepViewModel> GetCheeps();
        public List<CheepViewModel> GetCheepsFromAuthor(string author);
    }

    public class CheepService : ICheepService
    {
        private static readonly List<CheepViewModel> _cheeps = new();

        private DBFacade _db;
        public CheepService()
        {
            // Load cheeps from database
            _db = new DBFacade();

        }

        public List<CheepViewModel> GetCheeps()
        {
            return _db.GetCheeps();
        }

        public List<CheepViewModel> GetCheepsFromAuthor(string author)
        {
            return _db.GetCheepsFromAuthor(author);
        }

        private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime.ToString("MM/dd/yy H:mm:ss");
        }
    }
}