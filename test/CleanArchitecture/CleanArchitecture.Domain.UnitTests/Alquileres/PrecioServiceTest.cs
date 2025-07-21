using CleanArchitecture.Domain.Alquileres;
using CleanArchitecture.Domain.Shared;
using CleanArchitecture.Domain.Vehiculos;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Domain.UnitTests.Alquileres;

public class PrecioServiceTest
{
    private readonly PrecioService _precioService;

    public PrecioServiceTest()
    {
        _precioService = new PrecioService();
    }

    [Fact]
    public void CalcularPrecio_WithBasicVehiculo_ShouldCalculateCorrectPrecioPorPeriodo()
    {
        // Arrange
        var vehiculo = CreateBasicVehiculo();
        var periodo = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)));

        // Act
        var resultado = _precioService.CalcularPrecio(vehiculo, periodo);

        // Assert
        resultado.Should().NotBeNull();
        resultado.PrecioPorPeriodo.Monto.Should().Be(300m); // 100 * 3 días
        resultado.PrecioPorPeriodo.TipoMoneda.Should().Be(TipoMoneda.Usd);
        resultado.PrecioMantenimiento.Should().Be(vehiculo.Mantenimiento);
        resultado.PrecioAccesorios.Monto.Should().Be(0m); // Sin accesorios
        resultado.PrecioTotal.Monto.Should().Be(320m); // 300 + 20 mantenimiento
    }

    [Fact]
    public void CalcularPrecio_WithVehiculoWithAccesorios_ShouldCalculateCorrectAccesorioCharges()
    {
        // Arrange
        var vehiculo = CreateVehiculoWithAccesorios();
        var periodo = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)));

        // Act
        var resultado = _precioService.CalcularPrecio(vehiculo, periodo);

        // Assert
        resultado.Should().NotBeNull();
        resultado.PrecioPorPeriodo.Monto.Should().Be(200m); // 100 * 2 días
        resultado.PrecioAccesorios.Monto.Should().Be(12m); // 200 * 0.06 (AppleCar 5% + AireAcondicionado 1%)
        resultado.PrecioTotal.Monto.Should().Be(232m); // 200 + 20 + 12
    }

    [Fact]
    public void CalcularPrecio_WithVehiculoWithAllAccesorios_ShouldCalculateCorrectTotal()
    {
        // Arrange
        var vehiculo = CreateVehiculoWithAllAccesorios();
        var periodo = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        // Act
        var resultado = _precioService.CalcularPrecio(vehiculo, periodo);

        // Assert
        resultado.Should().NotBeNull();
        resultado.PrecioPorPeriodo.Monto.Should().Be(100m); // 100 * 1 día
        resultado.PrecioAccesorios.Monto.Should().Be(12m); // 100 * 0.12 (todos los accesorios)
        resultado.PrecioTotal.Monto.Should().Be(132m); // 100 + 20 + 12
    }

    [Fact]
    public void CalcularPrecio_WithVehiculoWithoutMantenimiento_ShouldNotAddMantenimientoToTotal()
    {
        // Arrange
        var vehiculo = CreateVehiculoWithoutMantenimiento();
        var periodo = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        // Act
        var resultado = _precioService.CalcularPrecio(vehiculo, periodo);

        // Assert
        resultado.Should().NotBeNull();
        resultado.PrecioMantenimiento.Monto.Should().Be(0m);
        resultado.PrecioTotal.Monto.Should().Be(100m); // Solo precio por periodo
    }

    [Fact]
    public void CalcularPrecio_WithDifferentTipoMoneda_ShouldUseCorrectTipoMoneda()
    {
        // Arrange
        var vehiculo = CreateVehiculoWithEur();
        var periodo = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)));

        // Act
        var resultado = _precioService.CalcularPrecio(vehiculo, periodo);

        // Assert
        resultado.Should().NotBeNull();
        resultado.PrecioPorPeriodo.TipoMoneda.Should().Be(TipoMoneda.Eur);
        resultado.PrecioMantenimiento.TipoMoneda.Should().Be(TipoMoneda.Eur);
        resultado.PrecioAccesorios.TipoMoneda.Should().Be(TipoMoneda.Eur);
        resultado.PrecioTotal.TipoMoneda.Should().Be(TipoMoneda.Eur);
    }

    [Fact]
    public void CalcularPrecio_WithLongPeriod_ShouldCalculateCorrectPrecioPorPeriodo()
    {
        // Arrange
        var vehiculo = CreateBasicVehiculo();
        var periodo = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)));

        // Act
        var resultado = _precioService.CalcularPrecio(vehiculo, periodo);

        // Assert
        resultado.Should().NotBeNull();
        resultado.PrecioPorPeriodo.Monto.Should().Be(700m); // 100 * 7 días
        resultado.PrecioTotal.Monto.Should().Be(720m); // 700 + 20 mantenimiento
    }

    [Fact]
    public void CalcularPrecio_WithSingleDayPeriod_ShouldCalculateCorrectPrecioPorPeriodo()
    {
        // Arrange
        var vehiculo = CreateBasicVehiculo();
        var periodo = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        var resultado = _precioService.CalcularPrecio(vehiculo, periodo);

        // Assert
        resultado.Should().NotBeNull();
        resultado.PrecioPorPeriodo.Monto.Should().Be(100m); // 100 * 1 día
        resultado.PrecioTotal.Monto.Should().Be(120m); // 100 + 20 mantenimiento
    }

    [Theory]
    [InlineData(Accesorio.AppleCar, 0.05)]
    [InlineData(Accesorio.AndroidCar, 0.05)]
    [InlineData(Accesorio.AireAcondicionado, 0.01)]
    [InlineData(Accesorio.Mapas, 0.01)]
    [InlineData(Accesorio.Wifi, 0.00)]
    public void CalcularPrecio_WithSingleAccesorio_ShouldCalculateCorrectPercentage(Accesorio accesorio, decimal expectedPercentage)
    {
        // Arrange
        var vehiculo = CreateVehiculoWithSingleAccesorio(accesorio);
        var periodo = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        // Act
        var resultado = _precioService.CalcularPrecio(vehiculo, periodo);

        // Assert
        resultado.Should().NotBeNull();
        var expectedAccesorioCharge = 100m * expectedPercentage;
        resultado.PrecioAccesorios.Monto.Should().Be(expectedAccesorioCharge);
    }

    [Fact]
    public void CalcularPrecio_WithMultipleAccesorios_ShouldSumAllPercentages()
    {
        // Arrange
        var vehiculo = CreateVehiculoWithMultipleAccesorios();
        var periodo = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        // Act
        var resultado = _precioService.CalcularPrecio(vehiculo, periodo);

        // Assert
        resultado.Should().NotBeNull();
        // AppleCar (5%) + AndroidCar (5%) + AireAcondicionado (1%) + Mapas (1%) = 12%
        var expectedAccesorioCharge = 100m * 0.12m;
        resultado.PrecioAccesorios.Monto.Should().Be(expectedAccesorioCharge);
    }

    [Fact]
    public void CalcularPrecio_WithZeroPrecioVehiculo_ShouldCalculateCorrectly()
    {
        // Arrange
        var vehiculo = CreateVehiculoWithZeroPrecio();
        var periodo = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        // Act
        var resultado = _precioService.CalcularPrecio(vehiculo, periodo);

        // Assert
        resultado.Should().NotBeNull();
        resultado.PrecioPorPeriodo.Monto.Should().Be(0m);
        resultado.PrecioAccesorios.Monto.Should().Be(0m);
        resultado.PrecioTotal.Monto.Should().Be(20m); // Solo mantenimiento
    }

    private static Vehiculo CreateBasicVehiculo()
    {
        return new Vehiculo(
            VehiculoId.New(),
            new Modelo("Toyota Camry"),
            new Vin("1HGBH41JXMN109186"),
            new Moneda(100, TipoMoneda.Usd),
            new Moneda(20, TipoMoneda.Usd),
            null,
            new List<Accesorio>(),
            new Direccion("123 Main St", "New York", "NY", "Manhattan", "USA")
        );
    }

    private static Vehiculo CreateVehiculoWithAccesorios()
    {
        return new Vehiculo(
            VehiculoId.New(),
            new Modelo("Toyota Camry"),
            new Vin("1HGBH41JXMN109186"),
            new Moneda(100, TipoMoneda.Usd),
            new Moneda(20, TipoMoneda.Usd),
            null,
            new List<Accesorio> { Accesorio.AppleCar, Accesorio.AireAcondicionado },
            new Direccion("123 Main St", "New York", "NY", "Manhattan", "USA")
        );
    }

    private static Vehiculo CreateVehiculoWithAllAccesorios()
    {
        return new Vehiculo(
            VehiculoId.New(),
            new Modelo("Toyota Camry"),
            new Vin("1HGBH41JXMN109186"),
            new Moneda(100, TipoMoneda.Usd),
            new Moneda(20, TipoMoneda.Usd),
            null,
            new List<Accesorio> { Accesorio.AppleCar, Accesorio.AndroidCar, Accesorio.AireAcondicionado, Accesorio.Mapas },
            new Direccion("123 Main St", "New York", "NY", "Manhattan", "USA")
        );
    }

    private static Vehiculo CreateVehiculoWithoutMantenimiento()
    {
        return new Vehiculo(
            VehiculoId.New(),
            new Modelo("Toyota Camry"),
            new Vin("1HGBH41JXMN109186"),
            new Moneda(100, TipoMoneda.Usd),
            Moneda.Zero(TipoMoneda.Usd),
            null,
            new List<Accesorio>(),
            new Direccion("123 Main St", "New York", "NY", "Manhattan", "USA")
        );
    }

    private static Vehiculo CreateVehiculoWithEur()
    {
        return new Vehiculo(
            VehiculoId.New(),
            new Modelo("Toyota Camry"),
            new Vin("1HGBH41JXMN109186"),
            new Moneda(100, TipoMoneda.Eur),
            new Moneda(20, TipoMoneda.Eur),
            null,
            new List<Accesorio>(),
            new Direccion("123 Main St", "New York", "NY", "Manhattan", "USA")
        );
    }

    private static Vehiculo CreateVehiculoWithSingleAccesorio(Accesorio accesorio)
    {
        return new Vehiculo(
            VehiculoId.New(),
            new Modelo("Toyota Camry"),
            new Vin("1HGBH41JXMN109186"),
            new Moneda(100, TipoMoneda.Usd),
            new Moneda(20, TipoMoneda.Usd),
            null,
            new List<Accesorio> { accesorio },
            new Direccion("123 Main St", "New York", "NY", "Manhattan", "USA")
        );
    }

    private static Vehiculo CreateVehiculoWithMultipleAccesorios()
    {
        return new Vehiculo(
            VehiculoId.New(),
            new Modelo("Toyota Camry"),
            new Vin("1HGBH41JXMN109186"),
            new Moneda(100, TipoMoneda.Usd),
            new Moneda(20, TipoMoneda.Usd),
            null,
            new List<Accesorio> { Accesorio.AppleCar, Accesorio.AndroidCar, Accesorio.AireAcondicionado, Accesorio.Mapas },
            new Direccion("123 Main St", "New York", "NY", "Manhattan", "USA")
        );
    }

    private static Vehiculo CreateVehiculoWithZeroPrecio()
    {
        return new Vehiculo(
            VehiculoId.New(),
            new Modelo("Toyota Camry"),
            new Vin("1HGBH41JXMN109186"),
            new Moneda(0, TipoMoneda.Usd),
            new Moneda(20, TipoMoneda.Usd),
            null,
            new List<Accesorio>(),
            new Direccion("123 Main St", "New York", "NY", "Manhattan", "USA")
        );
    }
}
