using System.Collections.Generic;

namespace Chirp.Razor;

public class PagedResults<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}