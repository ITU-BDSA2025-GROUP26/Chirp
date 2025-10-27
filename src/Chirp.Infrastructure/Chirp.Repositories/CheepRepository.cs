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
                Author = c.Author,
                TimeStamp = c.TimeStamp
            })
            .ToList();
            //return new List<CheepDto>();
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
                TimeStamp = c.TimeStamp
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
        cheep.TimeStamp = cheepdto.TimeStamp;
        cheep.Text = cheepdto.Text;

        if (!_context.Authors.Contains(cheepdto.Author))
        {
            AddAuthor(cheepdto.Author);
        }
        
        cheep.AuthorId = cheepdto.Author.AuthorId;
        cheep.CheepId = _context.Cheeps.Count() + 1;
        _context.Add(cheep);
        cheep.Author.Cheeps.Add(cheep);
    }

    //lave egen metode til at konvertere unix timestamp til datetime string
    
}