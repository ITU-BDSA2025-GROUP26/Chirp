namespace Chirp.Razor.Interfaces;

    public interface ICheepRepository
    {
        public void CreateCheep(CheepDto cheep);

        public List<CheepDto> ReadCheeps(string authorName);

        public void UpdateCheep(CheepDto alteredCheep);
    }