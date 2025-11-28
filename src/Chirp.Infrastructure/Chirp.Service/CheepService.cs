using Chirp.Core;
using Chirp.Core.Models;
using Chirp.Infrastructure.Chirp.Repositories;

namespace Chirp.Infrastructure.Chirp.Service
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
        
        public void AddCheep(string authorUserName, string text)
        {
            _cheepRepository.AddCheep(new CheepDto
            {
                Author = authorUserName,
                Text = text
                // TimeStamp will be set server-side in repository
            });
        }
        
        public void LikeCheep(int cheepId)          // NEW
        {
            _cheepRepository.LikeCheep(cheepId);
        }
    }
}