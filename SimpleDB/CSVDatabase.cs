namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int? limit = null)
    {
        return new String[1] { "Hello" };
    }

    public void Store(T record)
    {
        
    }
}
