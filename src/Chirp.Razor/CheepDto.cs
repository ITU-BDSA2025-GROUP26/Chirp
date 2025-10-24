using Chirp.Razor.Core.Models;

namespace Chirp.Razor;

public class CheepDto
{
    public string Text { get; set; }
    public Author Author { get; set; }
    public DateTime TimeStamp { get; set; } //make a string, method for turning into datetime
    
}