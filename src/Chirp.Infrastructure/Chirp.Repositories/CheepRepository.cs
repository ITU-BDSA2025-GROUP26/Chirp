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

    public CheepRepository(ChirpDBContext context)
    {
        _context = context;
    }

    public List<CheepDto> GetCheeps(int page, int pageSize)
    {
        return _context.Cheeps
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepDto
            {
                Text = c.Text,
                Author = c.Author,
                TimeStamp = c.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToList();
    }

    public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize)
    {
        return _context.Cheeps
            .Where(c => c.Author.Name == author)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepDto
            {
                Text = c.Text,
                Author = c.Author,
                TimeStamp = c.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")
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
            .Where(c => c.Author.Name == authorName)
            .OrderByDescending(c => c.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public Author GetAuthorByName(string authorName)
    {
        return _context.Authors
            .Single(a => a.Name == authorName);
    }

    public Author GetAuthorByEmail(string email)
    {
        return _context.Authors
            .Single(a => a.Email == email);
    }

    public void AddAuthor(string authorName, string email)
    {
        Author author = new Author();
        author.Name = authorName;
        author.Email = email;
        author.AuthorId = _context.Authors.Count() + 1;
        _context.Authors.Add(author);
    }

    public void AddAuthor(Author author)
    {
        _context.Authors.Add(author);
    }

    public void AddCheep(CheepDto cheepdto)
    {
        if (String.IsNullOrEmpty(cheepdto.Text))
        {
            throw new ArgumentException("You must write something in your cheep");
        } else if (cheepdto.Text.Length > 160)
        {
            throw new ArgumentException("Your cheep must not exceed 160 characters");
        }
        
        Cheep cheep = new Cheep();
        cheep.TimeStamp = StringTimeStampToDateTime(cheepdto.TimeStamp);
        cheep.Text = cheepdto.Text;

        var author = _context.Authors
            .SingleOrDefault(a => a.Name == cheepdto.Author.Name);

        if (author == null)
        {
            author = new Author
            {
                Name = cheepdto.Author.Name,
                Email = cheepdto.AuthorEmail,
                AuthorId = _context.Authors.Count() + 1,
                Cheeps = new List<Cheep>()
            };

            AddAuthor(author);
            _context.SaveChanges();
        }

        cheep.AuthorId = author.AuthorId;
        cheep.CheepId = _context.Cheeps.Count() + 1;
        _context.Add(cheep);
        cheep.Author.Cheeps.Add(cheep);
    }
    
    private static DateTime StringTimeStampToDateTime(string stringTimeStamp)
    {
        DateTime dateTime = DateTime.Parse(stringTimeStamp);
        return dateTime;
    }
    
}