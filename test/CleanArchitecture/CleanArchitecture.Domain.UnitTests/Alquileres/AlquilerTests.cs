using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Alquileres;
using CleanArchitecture.Domain.Alquileres.Events;
using CleanArchitecture.Domain.Shared;
using CleanArchitecture.Domain.UnitTests.Common;
using CleanArchitecture.Domain.Users;
using CleanArchitecture.Domain.Vehiculos;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Domain.UnitTests.Alquileres;

public class AlquilerTests
{
    [Fact]
    public void Reservar_WithValidParameters_ShouldCreateAlquilerWithCorrectProperties()
    {
        // Arrange
        var vehiculo = CreateTestVehiculo();
        var userId = UserId.New();
        var duracion = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)));
        var precioService = new PrecioService();
        var fechaCreacion = DateTime.UtcNow;

        // Act
        var alquiler = Alquiler.Reservar(vehiculo, userId, duracion, fechaCreacion, precioService);

        // Assert
        alquiler.Should().NotBeNull();
        alquiler.Id.Should().NotBeNull();
        alquiler.VehiculoId.Should().Be(vehiculo.Id);
        alquiler.UserId.Should().Be(userId);
        alquiler.Duracion.Should().Be(duracion);
        alquiler.Status.Should().Be(AlquilerStatus.Reservado);
        alquiler.FechaCreacion.Should().Be(fechaCreacion);
        alquiler.PrecioPorPeriodo.Should().NotBeNull();
        alquiler.PrecioMantenimiento.Should().NotBeNull();
        alquiler.PrecioAccesorios.Should().NotBeNull();
        alquiler.PrecioTotal.Should().NotBeNull();
    }

    [Fact]
    public void Reservar_WithValidParameters_ShouldGenerateUniqueAlquilerId()
    {
        // Arrange
        var vehiculo = CreateTestVehiculo();
        var userId = UserId.New();
        var duracion = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)));
        var precioService = new PrecioService();

        // Act
        var alquiler1 = Alquiler.Reservar(vehiculo, userId, duracion, DateTime.UtcNow, precioService);
        var alquiler2 = Alquiler.Reservar(vehiculo, userId, duracion, DateTime.UtcNow, precioService);

        // Assert
        alquiler1.Id.Should().NotBe(alquiler2.Id);
        alquiler1.Id.Value.Should().NotBeEmpty();
        alquiler2.Id.Value.Should().NotBeEmpty();
    }

    [Fact]
    public void Reservar_WithValidParameters_ShouldRaiseAlquilerReservadoDomainEvent()
    {
        // Arrange
        var vehiculo = CreateTestVehiculo();
        var userId = UserId.New();
        var duracion = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)));
        var precioService = new PrecioService();

        // Act
        var alquiler = Alquiler.Reservar(vehiculo, userId, duracion, DateTime.UtcNow, precioService);

        // Assert
        DomainEventAssertions.AssertDomainEventWasPublished<AlquilerReservadoDomainEvent, AlquilerId>(alquiler);

        var domainEvent = DomainEventAssertions.GetPublishedDomainEvent<AlquilerReservadoDomainEvent, AlquilerId>(alquiler);
        domainEvent.AlquilerId.Should().Be(alquiler.Id);
    }

    [Fact]
    public void Reservar_WithValidParameters_ShouldUpdateVehiculoFechaUltimoAlquiler()
    {
        // Arrange
        var vehiculo = CreateTestVehiculo();
        var userId = UserId.New();
        var duracion = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)));
        var precioService = new PrecioService();
        var fechaAntes = vehiculo.FechaUltimoAlquiler;

        // Act
        var alquiler = Alquiler.Reservar(vehiculo, userId, duracion, DateTime.UtcNow, precioService);

        // Assert
        vehiculo.FechaUltimoAlquiler.Should().NotBe(fechaAntes);
        vehiculo.FechaUltimoAlquiler.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Confirmar_WhenReservado_ShouldChangeStatusToConfirmado()
    {
        // Arrange
        var alquiler = CreateTestAlquiler();
        var utcNow = DateTime.UtcNow;

        // Act
        var result = alquiler.Confirmar(utcNow);

        // Assert
        result.Should().Be(Result.Success());
        alquiler.Status.Should().Be(AlquilerStatus.Confirmado);
        alquiler.FechaConfirmacion.Should().Be(utcNow);
    }

    [Fact]
    public void Confirmar_WhenReservado_ShouldRaiseAlquilerConfirmadoDomainEvent()
    {
        // Arrange
        var alquiler = CreateTestAlquiler();
        var utcNow = DateTime.UtcNow;

        // Act
        alquiler.Confirmar(utcNow);

        // Assert
        DomainEventAssertions.AssertDomainEventWasPublished<AlquilerConfirmadoDomainEvent, AlquilerId>(alquiler);

        var domainEvent = DomainEventAssertions.GetPublishedDomainEvent<AlquilerConfirmadoDomainEvent, AlquilerId>(alquiler);
        domainEvent.AlquilerId.Should().Be(alquiler.Id);
    }

    [Fact]
    public void Confirmar_WhenNotReservado_ShouldReturnFailure()
    {
        // Arrange
        var alquiler = CreateTestAlquiler();
        alquiler.Confirmar(DateTime.UtcNow); // Confirmar primero
        var utcNow = DateTime.UtcNow;

        // Act
        var result = alquiler.Confirmar(utcNow);

        // Assert
        result.Should().Be(Result.Failure(AlquilerErrors.NotReserved));
        alquiler.Status.Should().Be(AlquilerStatus.Confirmado); // Status no cambia
    }

    [Fact]
    public void Rechazar_WhenReservado_ShouldChangeStatusToRechazado()
    {
        // Arrange
        var alquiler = CreateTestAlquiler();
        var utcNow = DateTime.UtcNow;

        // Act
        var result = alquiler.Rechazar(utcNow);

        // Assert
        result.Should().Be(Result.Success());
        alquiler.Status.Should().Be(AlquilerStatus.Rechazado);
        alquiler.FechaDenegacion.Should().Be(utcNow);
    }

    [Fact]
    public void Rechazar_WhenReservado_ShouldRaiseAlquilerRechazadoDomainEvent()
    {
        // Arrange
        var alquiler = CreateTestAlquiler();
        var utcNow = DateTime.UtcNow;

        // Act
        alquiler.Rechazar(utcNow);

        // Assert
        DomainEventAssertions.AssertDomainEventWasPublished<AlquilerRechazadoDomainEvent, AlquilerId>(alquiler);

        var domainEvent = DomainEventAssertions.GetPublishedDomainEvent<AlquilerRechazadoDomainEvent, AlquilerId>(alquiler);
        domainEvent.AlquilerId.Should().Be(alquiler.Id);
    }

    [Fact]
    public void Rechazar_WhenNotReservado_ShouldReturnFailure()
    {
        // Arrange
        var alquiler = CreateTestAlquiler();
        alquiler.Confirmar(DateTime.UtcNow); // Confirmar primero
        var utcNow = DateTime.UtcNow;

        // Act
        var result = alquiler.Rechazar(utcNow);

        // Assert
        result.Should().Be(Result.Failure(AlquilerErrors.NotReserved));
        alquiler.Status.Should().Be(AlquilerStatus.Confirmado); // Status no cambia
    }

    [Fact]
    public void Completar_WhenConfirmado_ShouldChangeStatusToCompletado()
    {
        // Arrange
        var alquiler = CreateTestAlquiler();
        alquiler.Confirmar(DateTime.UtcNow);
        var utcNow = DateTime.UtcNow;

        // Act
        var result = alquiler.Completar(utcNow);

        // Assert
        result.Should().Be(Result.Success());
        alquiler.Status.Should().Be(AlquilerStatus.Completado);
        alquiler.FechaCompletado.Should().Be(utcNow);
    }

    [Fact]
    public void Completar_WhenConfirmado_ShouldRaiseAlquilerCompletadoDomainEvent()
    {
        // Arrange
        var alquiler = CreateTestAlquiler();
        alquiler.Confirmar(DateTime.UtcNow);
        var utcNow = DateTime.UtcNow;

        // Act
        alquiler.Completar(utcNow);

        // Assert
        DomainEventAssertions.AssertDomainEventWasPublished<AlquilerCompletadoDomainEvent, AlquilerId>(alquiler);

        var domainEvent = DomainEventAssertions.GetPublishedDomainEvent<AlquilerCompletadoDomainEvent, AlquilerId>(alquiler);
        domainEvent.AlquilerId.Should().Be(alquiler.Id);
    }

    [Fact]
    public void Completar_WhenNotConfirmado_ShouldReturnFailure()
    {
        // Arrange
        var alquiler = CreateTestAlquiler(); // Solo reservado
        var utcNow = DateTime.UtcNow;

        // Act
        var result = alquiler.Completar(utcNow);

        // Assert
        result.Should().Be(Result.Failure(AlquilerErrors.NotConfirmado));
        alquiler.Status.Should().Be(AlquilerStatus.Reservado); // Status no cambia
    }

    [Fact]
    public void Cancelar_WhenConfirmado_ShouldChangeStatusToCancelado()
    {
        // Arrange
        var alquiler = CreateTestAlquiler();
        alquiler.Confirmar(DateTime.UtcNow);
        var utcNow = DateTime.UtcNow;

        // Act
        var result = alquiler.Cancelar(utcNow);

        // Assert
        result.Should().Be(Result.Success());
        alquiler.Status.Should().Be(AlquilerStatus.Cancelado);
        alquiler.FechaCancelacion.Should().Be(utcNow);
    }

    [Fact]
    public void Cancelar_WhenConfirmado_ShouldRaiseAlquilerCanceladoDomainEvent()
    {
        // Arrange
        var alquiler = CreateTestAlquiler();
        alquiler.Confirmar(DateTime.UtcNow);
        var utcNow = DateTime.UtcNow;

        // Act
        alquiler.Cancelar(utcNow);

        // Assert
        DomainEventAssertions.AssertDomainEventWasPublished<AlquilerCanceladoDomainEvent, AlquilerId>(alquiler);

        var domainEvent = DomainEventAssertions.GetPublishedDomainEvent<AlquilerCanceladoDomainEvent, AlquilerId>(alquiler);
        domainEvent.AlquilerId.Should().Be(alquiler.Id);
    }

    [Fact]
    public void Cancelar_WhenNotConfirmado_ShouldReturnFailure()
    {
        // Arrange
        var alquiler = CreateTestAlquiler(); // Solo reservado
        var utcNow = DateTime.UtcNow;

        // Act
        var result = alquiler.Cancelar(utcNow);

        // Assert
        result.Should().Be(Result.Failure(AlquilerErrors.NotConfirmado));
        alquiler.Status.Should().Be(AlquilerStatus.Reservado); // Status no cambia
    }

    [Fact]
    public void Cancelar_WhenAlreadyStarted_ShouldReturnFailure()
    {
        // Arrange
        var alquiler = CreateTestAlquiler();
        alquiler.Confirmar(DateTime.UtcNow);

        // Crear un periodo que ya comenzó
        var duracionPasada = DateRange.Create(
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        );

        // Usar reflexión para cambiar la duración (solo para pruebas)
        var duracionField = typeof(Alquiler).GetField("Duracion",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        duracionField?.SetValue(alquiler, duracionPasada);

        var utcNow = DateTime.UtcNow;

        // Act
        var result = alquiler.Cancelar(utcNow);

        // Assert
        result.Should().Be(Result.Failure(AlquilerErrors.AlreadyStarted));
        alquiler.Status.Should().Be(AlquilerStatus.Confirmado); // Status no cambia
    }


    [Fact]
    public void Alquiler_EntityBase_ShouldInheritFromEntity()
    {
        // Arrange & Act
        var alquiler = CreateTestAlquiler();

        // Assert
        alquiler.Should().BeAssignableTo<Entity<AlquilerId>>();
        alquiler.Id.Should().BeOfType<AlquilerId>();
    }

    [Fact]
    public void Alquiler_Properties_ShouldBeReadOnlyFromOutside()
    {
        // Arrange & Act
        var alquiler = CreateTestAlquiler();

        // Assert
        alquiler.VehiculoId.Should().NotBeNull();
        alquiler.UserId.Should().NotBeNull();
        alquiler.Duracion.Should().NotBeNull();
        alquiler.PrecioPorPeriodo.Should().NotBeNull();
        alquiler.PrecioMantenimiento.Should().NotBeNull();
        alquiler.PrecioAccesorios.Should().NotBeNull();
        alquiler.PrecioTotal.Should().NotBeNull();
        alquiler.Status.Should().Be(AlquilerStatus.Reservado);
        alquiler.FechaCreacion.Should().NotBeNull();
    }

    [Theory]
    [InlineData(1, 100, 20, 12, 132)] // 1 día, precio 100, mantenimiento 20, accesorios 12
    [InlineData(3, 100, 20, 18, 338)] // 3 días, precio 100, mantenimiento 20, accesorios 18
    [InlineData(7, 100, 20, 42, 762)] // 7 días, precio 100, mantenimiento 20, accesorios 42
    public void Reservar_WithDifferentPeriods_ShouldCalculateCorrectPrecios(int dias, decimal precioBase, decimal mantenimiento, decimal accesorios, decimal totalEsperado)
    {
        // Arrange
        var vehiculo = CreateTestVehiculoWithCustomPrecio(precioBase, mantenimiento);
        var userId = UserId.New();
        var duracion = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(dias)));
        var precioService = new PrecioService();

        // Act
        var alquiler = Alquiler.Reservar(vehiculo, userId, duracion, DateTime.UtcNow, precioService);

        // Assert
        alquiler.PrecioPorPeriodo!.Monto.Should().Be(precioBase * dias);
        alquiler.PrecioMantenimiento!.Monto.Should().Be(mantenimiento);
        alquiler.PrecioAccesorios!.Monto.Should().Be(accesorios);
        alquiler.PrecioTotal!.Monto.Should().Be(totalEsperado);
    }

    private static Vehiculo CreateTestVehiculo()
    {
        return new Vehiculo(
            VehiculoId.New(),
            new Modelo("Toyota Camry"),
            new Vin("1HGBH41JXMN109186"),
            new Moneda(100, TipoMoneda.Usd),
            new Moneda(20, TipoMoneda.Usd),
            null,
            new List<Accesorio> { Accesorio.AireAcondicionado, Accesorio.Wifi },
            new Direccion("123 Main St", "New York", "NY", "Manhattan", "USA")
        );
    }

    private static Vehiculo CreateTestVehiculoWithCustomPrecio(decimal precio, decimal mantenimiento)
    {
        return new Vehiculo(
            VehiculoId.New(),
            new Modelo("Toyota Camry"),
            new Vin("1HGBH41JXMN109186"),
            new Moneda(precio, TipoMoneda.Usd),
            new Moneda(mantenimiento, TipoMoneda.Usd),
            null,
            new List<Accesorio> { Accesorio.AireAcondicionado, Accesorio.Wifi },
            new Direccion("123 Main St", "New York", "NY", "Manhattan", "USA")
        );
    }

    private static Alquiler CreateTestAlquiler()
    {
        var vehiculo = CreateTestVehiculo();
        var userId = UserId.New();
        var duracion = DateRange.Create(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)));
        var precioService = new PrecioService();

        return Alquiler.Reservar(vehiculo, userId, duracion, DateTime.UtcNow, precioService);
    }
}