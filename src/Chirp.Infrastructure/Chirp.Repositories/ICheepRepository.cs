using Chirp.Core;
using Chirp.Core.Models;
namespace Chirp.Infrastructure.Chirp.Repositories;

public interface ICheepRepository
{
    public List<CheepDto> GetCheeps(int page, int pageSize);
    public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize);
    public Author GetAuthorByName(string authorName);
    public Author GetAuthorByEmail(string email);
    public void AddAuthor(string authorName, string email);
    public void AddCheep(CheepDto cheepdto);
}