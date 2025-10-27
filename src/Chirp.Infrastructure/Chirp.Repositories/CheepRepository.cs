using Chirp.Core;
using Chirp.Core.Models;

namespace Chirp.Infrastructure.Chirp.Repositories;

public class CheepRepository:ICheepRepository
{
    private readonly ChirpDBContext _context;

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
                Author = c.Author.Name,
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
                Author = c.Author.Name,
                TimeStamp = c.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToList();

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
        Cheep cheep = new Cheep();
        cheep.TimeStamp = StringTimeStampToDateTime(cheepdto.TimeStamp);
        cheep.Text = cheepdto.Text;

        var author = _context.Authors
            .SingleOrDefault(a => a.Name == cheepdto.Author);

        if (author == null)
        {
            author = new Author
            {
                Name = cheepdto.Author,
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