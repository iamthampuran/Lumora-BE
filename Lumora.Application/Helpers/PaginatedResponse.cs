namespace Lumora.Application.Helpers;

public class PaginatedResponse<T>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int PageCount { get; set; } // Total number of items
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();

    public PaginatedResponse(IEnumerable<T> data, int totalCount, int currentPage, int pageSize)
    {
        Data = data;
        PageCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}