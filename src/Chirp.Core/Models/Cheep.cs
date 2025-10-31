using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Models;

public class Cheep
{
    public int CheepId { get; set; }
    [StringLength(160)]
    public string Text { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    
    public CheepDto ToDto()
    {
        CheepDto dto = new CheepDto();
        dto.Author =  this.Author;
        dto.TimeStamp = this.TimeStamp.ToLongDateString();
        dto.Text = this.Text;
        return dto;
    }
}