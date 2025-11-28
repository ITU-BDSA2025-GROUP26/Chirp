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

    public List<CheepDto> GetCheeps(int page, int pageSize)
    {
        var copenhagen = TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");
        
        return _context.Cheeps
            .OrderByDescending(c => c.TimeStamp) // sort newest first
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepDto
            {
                Text = c.Text,
                Author = c.Author!.UserName,
                TimeStamp = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.SpecifyKind(c.TimeStamp, DateTimeKind.Utc),
                    copenhagen
                ).ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToList();
    }

    public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize)
    {
        return _context.Cheeps
            .Where(c => c.Author.UserName == author)
            .OrderByDescending(c => c.TimeStamp) // sort newest first
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepDto
            {
                Text = c.Text,
                Author = c.Author!.UserName,
                TimeStamp = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.SpecifyKind(c.TimeStamp, DateTimeKind.Utc),
                    CopenhagenZone
                ).ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToList();
    }

    
    public Task<IReadOnlyList<Cheep>> GetCheepsFromPage(int page, CancellationToken ct = default)
        => GetCheepsFromPage(page, DefaultPageSize, ct);

    public async Task<IReadOnlyList<Cheep>> GetCheepsFromPage(int page, int pageSize, CancellationToken ct = default)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be >= 1.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be >= 1.");

        return await _context.Cheeps
            .OrderByDescending(c => c.TimeStamp) // or c.Timestamp
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);
    }
    
    public Task<IReadOnlyList<Cheep>> GetCheepsFromAuthorAndPage(string authorName, int page, CancellationToken ct = default)
        => GetCheepsFromAuthorAndPage(authorName, page, DefaultPageSize, ct);

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
}