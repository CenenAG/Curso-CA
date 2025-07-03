namespace CleanArchitecture.Domain.Shared;

public record PaginationParams
{
    public const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    public int _pageSize = 2;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string? OrderBy { get; set; }

    public bool OrdesAsc { get; set; } = true;

    public string? Search { get; set; }
}