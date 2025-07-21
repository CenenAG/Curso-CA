using System.Net.Http.Json;
using CleanArchitecture.Api.FunctionalTests.Users;
using CleanArchitecture.Application.Abstractions.Data;
using CleanArchitecture.Application.Users.RegisterUser;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;

namespace CleanArchitecture.Api.FunctionalTests.Infrastructure;

public class FuncionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
       .WithImage("postgres:16")
       .WithDatabase("kleanarchitecture")
       .WithUsername("postgres")
       .WithPassword("postgres")
       .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await CreateUserTestAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();


    }

    private async Task CreateUserTestAsync()
    {
        var httpClient = CreateClient();

        try
        {
            var response = await httpClient.PostAsJsonAsync("/api/v1/users/register", UserData.RegisterUserRequestTest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error creating test user: {response.StatusCode} - {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception creating test user: {ex.Message}");
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString())
                .UseSnakeCaseNamingConvention();
            });

            services.RemoveAll(typeof(ISqlConnectionFactory));
            services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(_dbContainer.GetConnectionString()));
        });

        base.ConfigureWebHost(builder);
    }
}