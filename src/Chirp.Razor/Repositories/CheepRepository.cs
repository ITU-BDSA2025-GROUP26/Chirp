using Chirp.Razor.Interfaces;
using Chirp.Razor.Data;

namespace Chirp.Razor.Repositories;

public class CheepRepository:ICheepRepository
{
    private readonly ChirpDBContext _context;

    public CheepRepository(ChirpDBContext context)
    {
        _context = context;
    }

    public List<CheepDto> GetCheeps(int page, int pageSize)
    {
        return _context.Cheeps
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepDto
            {
                Text = c.Text,
                Author = c.Author.Name,
                TimeStamp = c.TimeStamp.ToString("yyyy-MM-dd HH:mm") //convert to string
            })
            .ToList();
    }

    public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize)
    {
        return _context.Cheeps
            .Where(c => c.Author.Name == author)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepDto
            {
                Text = c.Text,
                Author = c.Author.Name,
                TimeStamp = c.TimeStamp.ToString("yyyy-MM-dd HH:mm") //convert to string
            })
            .ToList();

    }

    //lave egen metode til at konvertere unix timestamp til datetime string
    
}