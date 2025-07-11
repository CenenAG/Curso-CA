namespace CleanArchitecture.Infrastructure.Outbox;

public sealed class OutBoxMessage
{
    public OutBoxMessage(Guid id, DateTime ocurredOnUtc, string type, string content)
    {
        Id = id;
        OcurredOnUtc = ocurredOnUtc;
        Type = type;
        Content = content;
    }
    public Guid Id { get; init; }
    public DateTime OcurredOnUtc { get; init; }
    public string? Type { get; init; }
    public string? Content { get; init; }
    public DateTime? ProcessedOnUtc { get; init; }
    public string? Error { get; init; }
}