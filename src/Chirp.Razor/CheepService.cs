using Chirp.Razor.Pages;
using Chirp.Razor.Interfaces;

namespace Chirp.Razor
{
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


    }
}