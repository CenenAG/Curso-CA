namespace CleanArchitecture.Application.Users.GetUserSession;

public sealed class UserResponse
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public string? Nombre { get; init; }
    public string? Apellido { get; init; }

}