using CleanArchitecture.Domain.Abstractions;

namespace CleanArchitecture.Domain.Users;

public static class UserErrors
{
    public static Error NotFound = new("Users.NotFound", "The user was not found.");
    public static Error InvalidCredentials = new("Users.InvalidCredentials", "The user credentials are invalid.");
}