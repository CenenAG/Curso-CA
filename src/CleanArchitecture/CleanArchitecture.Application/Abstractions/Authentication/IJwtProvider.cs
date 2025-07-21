using CleanArchitecture.Domain.Users;

namespace CleanArchitecture.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken = default);
}