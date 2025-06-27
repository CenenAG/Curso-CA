using CleanArchitecture.Application.Abstractions.Messaging;

namespace CleanArchitecture.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string Nombre,
    string Apellido
) : ICommand<Guid>;
