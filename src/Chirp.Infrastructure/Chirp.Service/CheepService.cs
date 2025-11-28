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

        public Task Follow(string followerUserName, string followeeUserName) =>
            _cheepRepository.Follow(followerUserName, followeeUserName);

        public Task Unfollow(string followerUserName, string followeeUserName) =>
            _cheepRepository.Unfollow(followerUserName, followeeUserName);

        public Task<List<Author>> GetFollowing(string userNameOrEmail) =>
            _cheepRepository.GetFollowing(userNameOrEmail);
        
        public Task<List<Author>> GetFollowers(string userNameOrEmail) =>
            _cheepRepository.GetFollowers(userNameOrEmail);
    }
}