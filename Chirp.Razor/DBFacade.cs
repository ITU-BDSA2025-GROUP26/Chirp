namespace Chirp.Razor.Pages;

public class DBFacade
{
    private readonly string _path;

    private class DBFacade()
    {
        //_path = sti til database
        //var connection = new SqliteConnection() ???
    }
    
    public List<CheepViewModel> GetCheeps()
    {
        //connection.open()?
        return _cheeps;
    }
}