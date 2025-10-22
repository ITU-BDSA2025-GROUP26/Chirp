namespace Chirp.Razor.Models;

public class Author
{
    public string Name { get; set; }
    public string Email {get; set;}
    public int AuthorId { get; set;}
    public ICollection<Cheep> Cheeps {get; set;}
}