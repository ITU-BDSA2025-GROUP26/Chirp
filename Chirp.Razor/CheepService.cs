using Chirp.Razor.Pages;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    List<CheepViewModel> GetCheeps(int page, int pageSize);
    List<CheepViewModel> GetCheepsFromAuthor(string author, int page, int pageSize);
}

public class CheepService : ICheepService
{
    private readonly DBFacade _db;

    public CheepService()
    {
        // Default db path – could also be read from env var like CHIRPDBPATH
        _db = new DBFacade("../data/chirp.db");
    }

    public List<CheepViewModel> GetCheeps(int page, int pageSize)
        => _db.GetCheeps(page, pageSize);

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page, int pageSize)
        => _db.GetCheepsFromAuthor(author, page, pageSize);
}