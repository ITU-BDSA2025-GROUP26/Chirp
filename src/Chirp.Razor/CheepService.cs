using Chirp.Razor.Pages;

namespace  Chirp.Razor
{

    public record CheepViewModel(string Author, string Message, string Timestamp);

    public interface ICheepService
    {
        public List<CheepViewModel> GetCheeps(int page, int pageSize);
        public List<CheepViewModel> GetCheepsFromAuthor(string author, int page, int pageSize);
    }

    public class CheepService : ICheepService
    {
        private static readonly List<CheepViewModel> _cheeps = new();

        //private IDBFacade _db;
        private DBFacade _db;

        public CheepService(/*IDBFacade db*/)
        {
            // Load cheeps from database
            // _db = db;
            _db = new DBFacade();

        }

        public List<CheepViewModel> GetCheeps(int page, int pageSize)
        {
            return _db.GetCheeps(page, pageSize);
        }

        public List<CheepViewModel> GetCheepsFromAuthor(string author, int page, int pageSize)
        {
            return _db.GetCheepsFromAuthor(author, page, pageSize);
        }

        private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime.ToString("MM/dd/yy H:mm:ss");
        }
    }
}