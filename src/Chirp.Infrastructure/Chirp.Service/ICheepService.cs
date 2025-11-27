using Chirp.Core;
using Chirp.Core.Models;
namespace Chirp.Infrastructure.Chirp.Service;

public interface ICheepService
{
    public List<CheepDto> GetCheeps(int page, int pageSize);
    public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize);
    void AddCheep(string authorUserName, string text);

    public Task Follow(string followerUserName, string followeeUserName);
    public Task Unfollow(string followerUserName, string followeeUserName);
    public Task<List<Author>> GetFollowing(string userNameOrEmail);
    public Task<List<Author>> GetFollowers(string userNameOrEmail);
}