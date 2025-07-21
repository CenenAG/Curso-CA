using System.Net.Http.Json;
using CleanArchitecture.Api.FunctionalTests.Users;
using CleanArchitecture.Application.Users.LoginUser;
using Xunit;

namespace CleanArchitecture.Api.FunctionalTests.Infrastructure;

public abstract class BaseFunctionalTests : IClassFixture<FuncionalTestWebAppFactory>
{
    protected readonly HttpClient httpClient;

    public BaseFunctionalTests(FuncionalTestWebAppFactory factory)
    {
        httpClient = factory.CreateClient();
    }

    protected async Task<string> GetToken()
    {
        Console.WriteLine($"Attempting to login with email: {UserData.RegisterUserRequestTest.Email}");
        Console.WriteLine($"Password: {UserData.RegisterUserRequestTest.Password}");
        Console.WriteLine($"Request: {new LoginUserRequest(UserData.RegisterUserRequestTest.Email, UserData.RegisterUserRequestTest.Password)}");

        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/v1/users/login",
        new LoginUserRequest(
            UserData.RegisterUserRequestTest.Email,
            UserData.RegisterUserRequestTest.Password));

        Console.WriteLine($"Login response status: {response.StatusCode}");

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Login successful, token length: {token?.Length ?? 0}");
            return token;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Login failed: {errorContent}");
        }

        return string.Empty;
    }
}