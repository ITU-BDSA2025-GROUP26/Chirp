namespace Chirp.Razor.Chirp.Infrastructure.Chirp.Service;

public interface ICheepService
{
    public List<CheepDto> GetCheeps(int page, int pageSize);
    public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize);
}