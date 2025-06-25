using CleanArchitecture.Domain.Users;

namespace CleanArchitecture.Application.Authentication;

public interface IJwtProvider
{
    Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken = default);
}