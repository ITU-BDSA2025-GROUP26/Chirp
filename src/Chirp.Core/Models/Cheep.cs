namespace Chirp.Core.Models;

public class Cheep
{
    public int CheepId {get; set;}
    public string Text { get; set; }
    public DateTime TimeStamp { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
}