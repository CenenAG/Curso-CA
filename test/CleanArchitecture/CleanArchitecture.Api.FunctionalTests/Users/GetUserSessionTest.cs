using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CleanArchitecture.Api.FunctionalTests.Infrastructure;
using CleanArchitecture.Application.Users.GetUserSession;
using CleanArchitecture.Application.Users.LoginUser;
using CleanArchitecture.Application.Users.RegisterUser;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Xunit;

namespace CleanArchitecture.Api.FunctionalTests.Users;

public class GetUserSessionTest : BaseFunctionalTests
{
    public GetUserSessionTest(FuncionalTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Get_ShouldReturnUnAuthorized_WhenTokenIsMissing()
    {
        // act
        var response = await httpClient.GetAsync("/api/v1/users/me");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_ShouldReturnUser_WhenTokenExists()
    {
        // arrange
        Console.WriteLine("=== Starting Get_ShouldReturnUser_WhenTokenExists test ===");

        var token = await GetToken();
        Console.WriteLine($"Token obtained: {(!string.IsNullOrEmpty(token) ? "YES" : "NO")}");
        Console.WriteLine($"Token length: {token?.Length ?? 0}");

        if (!string.IsNullOrEmpty(token))
        {
            Console.WriteLine($"Token preview: {token.Substring(0, Math.Min(50, token.Length))}...");
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            token);

        // act
        Console.WriteLine("Making request to /api/v1/users/me...");
        var response = await httpClient.GetAsync("/api/v1/users/me");

        Console.WriteLine($"Response status: {response.StatusCode}");
        Console.WriteLine($"Response headers: {string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}"))}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error response: {errorContent}");
        }

        // assert
        response.Should().BeSuccessful();

        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();
        Console.WriteLine($"User response: {userResponse?.Email} - {userResponse?.Nombre} {userResponse?.Apellido}");

        userResponse.Should().NotBeNull();
        Console.WriteLine("=== Test completed successfully ===");
    }

    [Fact]
    public async Task Debug_StepByStep_UserCreationAndLogin()
    {
        Console.WriteLine("=== DEBUG TEST: Step by step user creation and login ===");

        // Step 1: Check if user exists
        Console.WriteLine("Step 1: Checking if test user exists...");
        var loginResponse = await httpClient.PostAsJsonAsync("/api/v1/users/login",
            new CleanArchitecture.Application.Users.LoginUser.LoginUserRequest(
                UserData.RegisterUserRequestTest.Email,
                UserData.RegisterUserRequestTest.Password));

        Console.WriteLine($"Initial login attempt status: {loginResponse.StatusCode}");

        if (!loginResponse.IsSuccessStatusCode)
        {
            var errorContent = await loginResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Initial login error: {errorContent}");

            // Step 2: Create user if login fails
            Console.WriteLine("Step 2: Creating test user...");
            var registerResponse = await httpClient.PostAsJsonAsync("/api/v1/users/register",
                UserData.RegisterUserRequestTest);

            Console.WriteLine($"Register response status: {registerResponse.StatusCode}");

            if (!registerResponse.IsSuccessStatusCode)
            {
                var registerError = await registerResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Register error: {registerError}");
            }
            else
            {
                Console.WriteLine("User created successfully");

                // Step 3: Try login again
                Console.WriteLine("Step 3: Trying login again...");
                loginResponse = await httpClient.PostAsJsonAsync("/api/v1/users/login",
                    new CleanArchitecture.Application.Users.LoginUser.LoginUserRequest(
                        UserData.RegisterUserRequestTest.Email,
                        UserData.RegisterUserRequestTest.Password));

                Console.WriteLine($"Second login attempt status: {loginResponse.StatusCode}");
            }
        }

        if (loginResponse.IsSuccessStatusCode)
        {
            var token = await loginResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Login successful! Token length: {token?.Length ?? 0}");

            // Step 4: Test the token
            Console.WriteLine("Step 4: Testing token with /api/v1/users/me...");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                token);

            var meResponse = await httpClient.GetAsync("/api/v1/users/me");
            Console.WriteLine($"/me endpoint status: {meResponse.StatusCode}");

            if (meResponse.IsSuccessStatusCode)
            {
                var userResponse = await meResponse.Content.ReadFromJsonAsync<UserResponse>();
                Console.WriteLine($"User data retrieved: {userResponse?.Email} - {userResponse?.Nombre} {userResponse?.Apellido}");
            }
            else
            {
                var errorContent = await meResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"/me endpoint error: {errorContent}");
            }
        }

        Console.WriteLine("=== DEBUG TEST COMPLETED ===");
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenUserExists()
    {
        // arrange
        var request = new LoginUserRequest(
            UserData.RegisterUserRequestTest.Email,
            UserData.RegisterUserRequestTest.Password);

        // act
        var response = await httpClient.PostAsJsonAsync("/api/v1/users/login", request);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRequestIsValid()
    {
        // arrange
        var request = new RegisterUserRequest(
            "zen@test.com",
            "Doe",
            "John",
            "Password123"
        );

        // act
        var response = await httpClient.PostAsJsonAsync("/api/v1/users/register", request);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        //var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();
        //Console.WriteLine($"User response Final: {userResponse?.Email} - {userResponse?.Nombre} {userResponse?.Apellido}");

        //userResponse.Should().NotBeNull();
    }
}