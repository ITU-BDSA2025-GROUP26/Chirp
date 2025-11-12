namespace Chirp.Core;

public class CheepDto
{
    public string Text { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string TimeStamp { get; set; } = string.Empty;//make a string, method for turning into datetime
    //public string AuthorEmail { get; set; } = string.Empty;
}