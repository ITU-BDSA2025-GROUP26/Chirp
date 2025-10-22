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
    public void CreateCheep(CheepDto cheep)
    {
        
    }

    public List<CheepDto> ReadCheeps(string authorName)
    {
        return new List<CheepDto>();
    }

    public void UpdateCheep(CheepDto alteredCheep)
    {
        
    }
    
}