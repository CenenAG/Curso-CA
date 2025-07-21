using CleanArchitecture.ArchitectureTests.Infrastructure;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace CleanArchitecture.ArchitectureTests.Layers;

public class LayerTests : BaseTest
{
    [Fact]
    public void DomainLayer_Should_NotHaveDependencyOn_ApplicationLayer()
    {
        var resultados = Types.InAssembly(DomainAssembly)
        .Should()
        .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
        .GetResult();

        resultados.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainLayer_Should_NotHaveDependencyOn_InfrastructureLayer()
    {
        var resultados = Types.InAssembly(DomainAssembly)
        .Should()
        .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
        .GetResult();

        resultados.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ApplicationLayer_Should_NotHaveDependencyOn_InfrastructureLayer()
    {
        var resultados = Types.InAssembly(ApplicationAssembly)
        .Should()
        .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
        .GetResult();

        resultados.IsSuccessful.Should().BeTrue();
    }


    [Fact]
    public void ApplicationLayer_Should_NotHaveDependencyOn_PresentationLayer()
    {
        var resultados = Types.InAssembly(ApplicationAssembly)
        .Should()
        .NotHaveDependencyOn(PresentacionAssembly.GetName().Name)
        .GetResult();

        resultados.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void InfrastructureLayer_Should_NotHaveDependencyOn_PresentationLayer()
    {
        var resultados = Types.InAssembly(InfrastructureAssembly)
        .Should()
        .NotHaveDependencyOn(PresentacionAssembly.GetName().Name)
        .GetResult();

        resultados.IsSuccessful.Should().BeTrue();
    }
}