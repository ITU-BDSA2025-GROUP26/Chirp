namespace Chirp.Razor.Interfaces;

    public interface ICheepRepository
    {
        public Task CreateCheep(CheepDto cheep);

        public Task<List<CheepDto>> ReadCheeps(string authorName);

        public Task UpdateCheep(CheepDto alteredCheep);
        
        public List<CheepDto> GetCheeps(int page, int pageSize);
        public List<CheepDto> GetCheepsFromAuthor(string author, int page, int pageSize);
    }