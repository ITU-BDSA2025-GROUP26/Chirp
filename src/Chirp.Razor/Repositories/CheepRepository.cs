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
    public Task CreateCheep(CheepDto cheep)
    {
        

    }

    public Task<List<CheepDto>> ReadCheeps(string authorName)
    {
        
    }

    public Task UpdateCheep(CheepDto alteredCheep)
    {
        
    }

    public List<CheepDto> GetCheeps(int page, int pageSize)
    {
        return new List<CheepDto>();
    }

    public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize)
    {
        
    }
    
}