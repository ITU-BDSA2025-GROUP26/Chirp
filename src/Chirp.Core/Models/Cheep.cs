using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Models;

public class Cheep
{
    public int CheepId { get; set; }
    [StringLength(160)]
    public string Text { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public Author Author { get; set; } = null!;
}
