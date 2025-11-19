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
        
        public Author GetAuthorByName(string authorUserName)
        {
            return _cheepRepository.GetAuthorByName(authorUserName);
        }

        public void Follow(Author follower, string followee)
        {
            Follow follow = new Follow
            {
                Follower = follower,
                FollowerId = follower.Id,
                Followee = _cheepRepository.GetAuthorByName(followee)
            };

            follow.FolloweeId = follow.Followee.Id;
            
            follower.Following.Add(follow);
            follow.Followee.Followers.Add(follow);
        }

        public void Unfollow(Author follower, string followee)
        {
            Follow follow = new Follow
            {
                Follower = follower,
                FollowerId = follower.Id,
                Followee = _cheepRepository.GetAuthorByName(followee)
            };
            follow.Followee.Followers.Remove(follow);
            follow.Follower.Following.Remove(follow);
        }

    }
}