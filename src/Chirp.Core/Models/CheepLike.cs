namespace Chirp.Core.Models;

public class CheepLike
{
    public int CheepId { get; set; }
    public Cheep Cheep { get; set; } = null!;

    public string AuthorId { get; set; } = string.Empty;  // who liked
    public Author Author { get; set; } = null!;
}