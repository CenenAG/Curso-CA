namespace CleanArchitecture.Domain.Abstractions;

public class PagedDapperResults<TEntity>
{
    public PagedDapperResults(
        int totalItems,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        var mod = totalItems % pageSize;
        var totalPages = (totalItems / pageSize) + (mod > 0 ? 1 : 0);

        TotalItems = totalItems;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = totalPages;

    }
    public IEnumerable<TEntity>? Items { get; set; }
    public int TotalItems { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }

}