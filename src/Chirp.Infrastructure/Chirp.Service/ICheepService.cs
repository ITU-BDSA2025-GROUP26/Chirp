using Chirp.Core;
using Chirp.Core.Models;
namespace Chirp.Infrastructure.Chirp.Service;

public interface ICheepService
{
    public List<CheepDto> GetCheeps(int page, int pageSize);
    public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize);
    public void AddCheep(string authorUserName, string text);
    public void LikeCheep(string authorUserName, int cheepId);
}