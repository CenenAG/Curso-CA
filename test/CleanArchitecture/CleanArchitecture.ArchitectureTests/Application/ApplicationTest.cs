using CleanArchitecture.Application.Abstractions.Messaging;
using CleanArchitecture.ArchitectureTests.Infrastructure;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace CleanArchitecture.ArchitectureTests.Application;

public class ApplicationTest : BaseTest
{
    [Fact]
    public void CommandsHandler_Shoul_NotBePublic()
    {
        var resultados = Types.InAssembly(ApplicationAssembly)
        .That()
        .ImplementInterface(typeof(ICommandHandler<>))
        .Or()
        .ImplementInterface(typeof(ICommandHandler<,>))
        .Should()
        .NotBePublic()
        .GetResult();


        resultados.IsSuccessful.Should().BeTrue();
    }


    [Fact]
    public void QueyHandler_Shoul_NotBePublic()
    {
        var resultados = Types.InAssembly(ApplicationAssembly)
        .That()
        .ImplementInterface(typeof(IQueryHandler<,>))
        .Should()
        .NotBePublic()
        .GetResult();

        //imprimir en consola resultados
        Console.WriteLine("resultados : " + resultados.ToString());

        resultados.IsSuccessful.Should().BeTrue();
    }
}