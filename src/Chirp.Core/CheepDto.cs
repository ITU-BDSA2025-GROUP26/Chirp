using Chirp.Core.Models;

namespace Chirp.Core;

public class CheepDto
{
    public string Text { get; set; } = string.Empty;
    public Author Author { get; set; }
    public string TimeStamp { get; set; } = string.Empty;//make a string, method for turning into datetime
    public string AuthorEmail { get; set; } = string.Empty;
}