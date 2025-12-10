using Chirp.Core.Models;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

public sealed class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDBContext _context;

    public AuthorRepository(ChirpDBContext context)
    {
        _context = context;
    }

    public Author GetAuthorByName(string authorName)
    {
        return _context.Authors
                   .Include(a => a.Following)
                   .Include(a => a.Followers)
                   .FirstOrDefault(a => a.UserName == authorName)
               ?? throw new InvalidOperationException($"Author '{authorName}' not found.");
    }

    public Author GetAuthorByEmail(string email)
    {
        return _context.Authors
                   .Include(a => a.Following)
                   .Include(a => a.Followers)
                   .FirstOrDefault(a => a.Email == email)
               ?? throw new InvalidOperationException($"Author with email '{email}' not found.");
    }

    public void AddAuthor(string authorName, string email)
    {
        var author = new Author
        {
            UserName = authorName,
            Email = email
        };

        _context.Authors.Add(author);
        _context.SaveChanges();
    }

    /// <summary>
    /// Method for one author to follow another author
    /// Checks for self-follow and prevents it and prevents duplicate follows
    /// </summary>
    /// <param name="followerUserName"></param>
    /// <param name="followeeUserName"></param>
    /// <returns></returns>
    public async Task Follow(string followerUserName, string followeeUserName)
    {
        if (string.Equals(followerUserName, followeeUserName, StringComparison.OrdinalIgnoreCase))
            return; // no self-follow

        var follower = await _context.Authors
            .Include(a => a.Following)
            .FirstOrDefaultAsync(a => a.UserName == followerUserName);

        var followee = await _context.Authors
            .Include(a => a.Followers)
            .FirstOrDefaultAsync(a => a.UserName == followeeUserName);

        if (follower == null || followee == null)
            return;

        if (!follower.Following.Contains(followee))
        {
            follower.Following.Add(followee);
            followee.Followers.Add(follower);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Method for one author to unfollow another author
    /// Prevents unfollowing of null authors and only allows unfollowing if currently following
    /// </summary>
    /// <param name="followerUserName"></param>
    /// <param name="followeeUserName"></param>
    /// <returns></returns>
    public async Task Unfollow(string followerUserName, string followeeUserName)
    {
        var follower = await _context.Authors
            .Include(a => a.Following)
            .FirstOrDefaultAsync(a => a.UserName == followerUserName);

        var followee = await _context.Authors
            .Include(a => a.Followers)
            .FirstOrDefaultAsync(a => a.UserName == followeeUserName);

        if (follower == null || followee == null)
            return;

        if (follower.Following.Contains(followee))
        {
            follower.Following.Remove(followee);
            followee.Followers.Remove(follower);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Author>> GetFollowing(string userNameOrEmail)
    {
        var author = await _context.Authors
            .Include(a => a.Following)
            .FirstOrDefaultAsync(a => a.UserName == userNameOrEmail || a.Email == userNameOrEmail);

        return author?.Following.ToList() ?? new List<Author>();
    }

    public async Task<List<Author>> GetFollowers(string userNameOrEmail)
    {
        var author = await _context.Authors
            .Include(a => a.Followers)
            .FirstOrDefaultAsync(a => a.UserName == userNameOrEmail || a.Email == userNameOrEmail);

        return author?.Followers.ToList() ?? new List<Author>();
    }
}