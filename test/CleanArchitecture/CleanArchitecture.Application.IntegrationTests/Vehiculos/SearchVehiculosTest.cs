using CleanArchitecture.Application.IntegrationTests.Infrastructure;
using CleanArchitecture.Application.Vehiculos.SearchVehiculos;
using CleanArchitecture.Domain.Vehiculos;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.Vehiculos;

public class SearchVehiculosTest : BaseIntegrationTest
{
    public SearchVehiculosTest(IntegrationTestWebAppFactory factory) : base(factory)
    {


    }

    [Fact]
    public async Task SearchVehiculos_ShouldReturnEmptyList_WhenDateRangeInvalis()
    {
        // Arrange
        var query = new SearchVehiculosQuery(new DateOnly(2023, 1, 1), new DateOnly(2022, 1, 1));

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchVehiculos_ShouldReturnVehiculos_WhenDateRangeIsValid()
    {
        // Arrange
        var query = new SearchVehiculosQuery(new DateOnly(2022, 1, 1), new DateOnly(2023, 1, 31));

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Value.Should().NotBeEmpty();
    }
}