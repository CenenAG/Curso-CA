using CleanArchitecture.Application.Users.RegisterUser;

namespace CleanArchitecture.Api.FunctionalTests.Users;

internal static class UserData
{
    public static readonly RegisterUserRequest RegisterUserRequestTest = new(
        "test@test.com",
        "Doe",
        "John",
        "Password123"
    );

}