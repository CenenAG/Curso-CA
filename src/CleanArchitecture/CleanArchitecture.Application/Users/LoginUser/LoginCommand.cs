using CleanArchitecture.Application.Abstractions.Messaging;

namespace CleanArchitecture.Application.Users.LoginUser;

public sealed record LoginCommand(
    string Email,
    string Password
) : ICommand<string>;
