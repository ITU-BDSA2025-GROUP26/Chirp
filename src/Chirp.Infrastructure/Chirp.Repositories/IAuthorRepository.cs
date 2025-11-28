using Chirp.Core.Models;

namespace Chirp.Infrastructure.Chirp.Repositories;

public interface IAuthorRepository
{
    // Author lookup / creation
    Author GetAuthorByName(string authorName);
    Author GetAuthorByEmail(string email);
    void AddAuthor(string authorName, string email);

    // Follow graph operations
    Task Follow(string followerUserName, string followeeUserName);
    Task Unfollow(string followerUserName, string followeeUserName);

    Task<List<Author>> GetFollowing(string userNameOrEmail);
    Task<List<Author>> GetFollowers(string userNameOrEmail);
}