using CleanArchitecture.Application.Abstractions.Clock;

namespace CleanArchitecture.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime CurrenTime => DateTime.UtcNow;
}
