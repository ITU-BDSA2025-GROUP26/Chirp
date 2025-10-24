namespace Chirp.Razor.Core.Models;

public class Cheep
{
    public int CheepId {get; set;}
    public string Text { get; set; }
    public DateTime TimeStamp { get; set; }
    public int AuthorId { get; set; }
    public Author Author {get; set;}

    public CheepDto ToDto()
    {
        CheepDto dto = new CheepDto();
        dto.Author =  this.Author;
        dto.TimeStamp = this.TimeStamp;
        dto.Text = this.Text;
        return dto;
    }
}