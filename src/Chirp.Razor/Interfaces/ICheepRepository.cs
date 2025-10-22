namespace Chirp.Razor.Interfaces;

    public interface ICheepRepository
    {
        public List<CheepDto> GetCheeps(int page, int pageSize);
        public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize);
    }