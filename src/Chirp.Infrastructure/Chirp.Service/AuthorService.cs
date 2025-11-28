using Chirp.Core.Models;
using Chirp.Infrastructure.Chirp.Repositories;

namespace Chirp.Infrastructure.Chirp.Service;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public Author GetAuthorByName(string authorName)
        => _authorRepository.GetAuthorByName(authorName);

    public Author GetAuthorByEmail(string email)
        => _authorRepository.GetAuthorByEmail(email);

    public void AddAuthor(string authorName, string email)
        => _authorRepository.AddAuthor(authorName, email);

    public Task Follow(string followerUserName, string followeeUserName)
        => _authorRepository.Follow(followerUserName, followeeUserName);

    public Task Unfollow(string followerUserName, string followeeUserName)
        => _authorRepository.Unfollow(followerUserName, followeeUserName);

    public Task<List<Author>> GetFollowing(string userNameOrEmail)
        => _authorRepository.GetFollowing(userNameOrEmail);

    public Task<List<Author>> GetFollowers(string userNameOrEmail)
        => _authorRepository.GetFollowers(userNameOrEmail);
}