using Chirp.Core;
using Chirp.Core.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

public sealed class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _context;
    private const int DefaultPageSize = 32;
    private static readonly TimeZoneInfo CopenhagenZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");

    public CheepRepository(ChirpDBContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get cheeps with pagination and sorts newest cheeps first
    /// Ensures Author is loaded
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public List<CheepDto> GetCheeps(int page, int pageSize)
    {
     var cheeps = _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return cheeps
            .Select(c => new CheepDto
            {
                CheepId = c.CheepId,
                Text = c.Text ?? string.Empty,
                Author = c.Author!.UserName ?? "(unknown)",
                TimeStamp = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.SpecifyKind(c.TimeStamp, DateTimeKind.Utc),
                    CopenhagenZone
                ).ToString("yyyy-MM-dd HH:mm:ss"),
                Likes = c.Likes
            })
            .ToList();
    }

    /// <summary>
    /// Get cheeps from a specific author with pagination and sorts newest cheeps first
    /// Ensures Author is loaded
    /// </summary>
    /// <param name="author"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize)
    {
        var cheeps = _context.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author != null && c.Author.UserName == author)
            .OrderByDescending(c => c.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return cheeps
            .Select(c => new CheepDto
            {
                CheepId = c.CheepId,
                Text = c.Text ?? string.Empty,
                Author = c.Author!.UserName ?? "(unknown)",
                TimeStamp = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.SpecifyKind(c.TimeStamp, DateTimeKind.Utc),
                    CopenhagenZone
                ).ToString("yyyy-MM-dd HH:mm:ss"),
                Likes = c.Likes
            })
            .ToList();
    }

    /// <summary>
    /// Get cheeps with pagination and sorts newest cheeps first
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task<IReadOnlyList<Cheep>> GetCheepsFromPage(int page, int pageSize, CancellationToken ct = default)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be >= 1.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be >= 1.");

        return await _context.Cheeps
            .OrderByDescending(c => c.TimeStamp) 
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    /// <summary>
    /// Get cheeps from a specific author with pagination and sorts newest cheeps first
    /// </summary>
    /// <param name="authorName"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task<IReadOnlyList<Cheep>> GetCheepsFromAuthorAndPage(string authorName, int page, int pageSize, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(authorName)) throw new ArgumentException("Author name is required.", nameof(authorName));
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be >= 1.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be >= 1.");

        return await _context.Cheeps
            .Where(c => c.Author!.UserName == authorName)
            .OrderByDescending(c => c.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    /// <summary>
    /// Adds a new cheep and checks for empty strings and max length, also checks that author exists
    /// </summary>
    /// <param name="cheepdto"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddCheep(CheepDto cheepdto)
    {
        if (cheepdto is null) throw new ArgumentNullException(nameof(cheepdto));

        var text = cheepdto.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("You must write something in your cheep", nameof(cheepdto));

        if (text.Length > 160)
            throw new ArgumentException("Your cheep must not exceed 160 characters", nameof(cheepdto));

        if (string.IsNullOrWhiteSpace(cheepdto.Author))
            throw new ArgumentException("Author (user name) is required.", nameof(cheepdto));

        
        var author = _context.Authors.SingleOrDefault(a => a.UserName == cheepdto.Author);
        if (author is null)
            throw new InvalidOperationException($"Author ' {cheepdto.Author} not found");
                
    
        var cheep = new Cheep
        {
            Text = text,
            TimeStamp = DateTime.UtcNow,
            AuthorId = author.Id,
        };

        _context.Cheeps.Add(cheep);
        _context.SaveChanges();
    }
    
    /// <summary>
    /// Like or unlike a cheep by a specific author
    /// Prevents multiple likes and unlikes by the same author on the same cheep
    /// </summary>
    /// <param name="authorUserName"></param>
    /// <param name="cheepId"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public void LikeCheep(string authorUserName, int cheepId)
    {
        if (string.IsNullOrWhiteSpace(authorUserName))
            throw new ArgumentException("Author user name is required.", nameof(authorUserName));

        var author = _context.Authors.SingleOrDefault(a => a.UserName == authorUserName)
                     ?? throw new InvalidOperationException($"Author '{authorUserName}' not found.");

        var cheep = _context.Cheeps.SingleOrDefault(c => c.CheepId == cheepId)
                    ?? throw new InvalidOperationException($"Cheep with id {cheepId} not found.");

        var existingLike = _context.CheepLikes
            .SingleOrDefault(cl => cl.CheepId == cheepId && cl.AuthorId == author.Id);

        if (existingLike is null)
        {
            var like = new CheepLike
            {
                CheepId = cheepId,
                AuthorId = author.Id
            };

            _context.CheepLikes.Add(like);
            cheep.Likes++;
        }
        else
        {
            _context.CheepLikes.Remove(existingLike);
            if (cheep.Likes > 0)
                cheep.Likes--;
        }

        _context.SaveChanges();
    }
}