namespace CleanArchitecture.Domain.Shared;

public record SpecificationEntry
{
    public string? Sort { get; init; }
    public int PageIndex { get; init; } = 1;

    private const int _maxPageSize = 50;
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > _maxPageSize ? _maxPageSize : value;
    }

    public string? Search { get; set; }
}
