using Chirp.Razor.Pages;

namespace Chirp.Razor
{

    public record CheepViewModel(string Author, string Message, string Timestamp);

    public interface ICheepService
    {
        public List<CheepDto> GetCheeps(int page, int pageSize);
        public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize);
    }

    public class CheepService : ICheepService
    {
        private static readonly List<CheepDto> _cheeps = new();

        private readonly ICheepRepository _cheepRepository;


        public CheepService(ICheepRepository cheepRepository)
        {
            _cheepRepository = cheepRepository;

        }

        public List<CheepDto> GetCheeps(int page, int pageSize)
        {
            return _cheepRepository.GetCheeps(page, pageSize);
        }

        public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize)
        {
            return _cheepRepository.GetCheepsFromAuthor(author, page, pageSize);
        }

        private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime.ToString("MM/dd/yy H:mm:ss");
        }

    }
}