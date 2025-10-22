namespace Chirp.Razor
{
    public class Author
    {
        public string Name { get; set; }
        public string Email {get; set;}
        public int AuthorId { get; set;}
        public ICollection<Cheep> Cheeps {get; set;}
    }

    public class Cheep
    {
        public int CheepId {get; set;}
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
        public int AuthorId { get; set; }
        public Author Author {get; set;}
    }

    public class CheepDto
    {
        public string Text { get; set; }
        public string Author { get; set; }
        public int UnixTimeStamp { get; set; } //how to format ???

    }
        
}