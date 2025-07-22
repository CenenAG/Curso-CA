using CleanArchitecture.Infrastructure;
using CleanArchitecture.Application;
using CleanArchitecture.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using CleanArchitecture.Api.OptionsSetup;
using CleanArchitecture.Infrastructure.Authentication;
using Serilog;
using CleanArchitecture.Api.Documentation;
using CleanArchitecture.Api.Controllers.Alquileres;
using Asp.Versioning.Builder;
using Asp.Versioning;
using CleanArchitecture.Application.Abstractions.Authentication;
using CleanArchitecture.Infrastructure.Email;
using CleanArchitecture.Application.Abstractions.Email;
using QuestPDF;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

builder.Services.Configure<GmailSettings>(builder.Configuration.GetSection("GmailSettings"));

builder.Services.AddTransient<IJwtProvider, JwtProvider>();

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddSwaggerGen(options =>
    {
        options.CustomSchemaIds(type => type.ToString());
    });

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Hello World");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}

// app.UseHttpsRedirection();

await app.ApplyMigration();
app.SeedData();
app.SeedDataAuthentication();



app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseCustomExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ApiVersionSet apiVersion = app.NewApiVersionSet()
                                    .HasApiVersion(new ApiVersion(1))
                                    .ReportApiVersions()
                                    .Build();

var routeGroupBuilder = app
                        .MapGroup("/api/v{version:apiVersion}")
                        .WithApiVersionSet(apiVersion);

routeGroupBuilder.MapAlquilerEndPoints();

app.Run();

public partial class Program;
