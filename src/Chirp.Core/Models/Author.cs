namespace Chirp.Core.Models;

public class Author
{
    public string Name { get; set; } = String.Empty;
    public string Email {get; set;} = String.Empty;
    public int AuthorId { get; set;}
    public List <Cheep> Cheeps { get; set;} = new();
}