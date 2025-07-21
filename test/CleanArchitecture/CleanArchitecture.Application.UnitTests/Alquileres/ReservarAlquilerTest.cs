using CleanArchitecture.Application.Abstractions.Clock;
using CleanArchitecture.Application.Alquileres.ReservarAlquiler;
using CleanArchitecture.Application.Exceptions;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Alquileres;
using CleanArchitecture.Domain.Alquileres.Events;
using CleanArchitecture.Domain.Roles;
using CleanArchitecture.Domain.Shared;
using CleanArchitecture.Domain.Users;
using CleanArchitecture.Domain.Vehiculos;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.Alquileres;

public class ReservarAlquilerTest
{
    private readonly IUserRepository _userRepository;
    private readonly IVehiculoRepository _vehiculoRepository;
    private readonly IAlquilerRepository _alquilerRepository;
    private readonly PrecioService _precioService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ReservarAlquilerCommandHandler _handler;

    public ReservarAlquilerTest()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _vehiculoRepository = Substitute.For<IVehiculoRepository>();
        _alquilerRepository = Substitute.For<IAlquilerRepository>();
        _precioService = new PrecioService();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new ReservarAlquilerCommandHandler(
            _userRepository,
            _vehiculoRepository,
            _alquilerRepository,
            _precioService,
            _unitOfWork,
            _dateTimeProvider);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnSuccessWithAlquilerId()
    {
        // Arrange
        var command = CreateValidCommand();
        var user = CreateTestUser();
        var vehiculo = CreateTestVehiculo();
        var currentTime = DateTime.UtcNow;

        _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _vehiculoRepository.GetByIdAsync(Arg.Any<VehiculoId>(), Arg.Any<CancellationToken>())
            .Returns(vehiculo);
        _alquilerRepository.IsOverLappingAsync(Arg.Any<Vehiculo>(), Arg.Any<DateRange>(), Arg.Any<CancellationToken>())
            .Returns(false);
        _dateTimeProvider.CurrenTime.Returns(currentTime);
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _alquilerRepository.Received(1).Add(Arg.Any<Alquiler>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var command = CreateValidCommand();

        _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound);
        await _vehiculoRepository.DidNotReceive().GetByIdAsync(Arg.Any<VehiculoId>(), Arg.Any<CancellationToken>());
        await _alquilerRepository.DidNotReceive().IsOverLappingAsync(Arg.Any<Vehiculo>(), Arg.Any<DateRange>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenVehiculoNotFound_ShouldReturnVehiculoNotFoundError()
    {
        // Arrange
        var command = CreateValidCommand();
        var user = CreateTestUser();

        _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _vehiculoRepository.GetByIdAsync(Arg.Any<VehiculoId>(), Arg.Any<CancellationToken>())
            .Returns((Vehiculo?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(VehiculoErrors.NotFound);
        await _alquilerRepository.DidNotReceive().IsOverLappingAsync(Arg.Any<Vehiculo>(), Arg.Any<DateRange>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOverlappingAlquiler_ShouldReturnOverlapError()
    {
        // Arrange
        var command = CreateValidCommand();
        var user = CreateTestUser();
        var vehiculo = CreateTestVehiculo();

        _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _vehiculoRepository.GetByIdAsync(Arg.Any<VehiculoId>(), Arg.Any<CancellationToken>())
            .Returns(vehiculo);
        _alquilerRepository.IsOverLappingAsync(Arg.Any<Vehiculo>(), Arg.Any<DateRange>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AlquilerErrors.Overlap);
        _alquilerRepository.DidNotReceive().Add(Arg.Any<Alquiler>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenConcurrencyException_ShouldReturnConcurrencyError()
    {
        // Arrange
        var command = CreateValidCommand();
        var user = CreateTestUser();
        var vehiculo = CreateTestVehiculo();
        var currentTime = DateTime.UtcNow;

        _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _vehiculoRepository.GetByIdAsync(Arg.Any<VehiculoId>(), Arg.Any<CancellationToken>())
            .Returns(vehiculo);
        _alquilerRepository.IsOverLappingAsync(Arg.Any<Vehiculo>(), Arg.Any<DateRange>(), Arg.Any<CancellationToken>())
            .Returns(false);
        _dateTimeProvider.CurrenTime.Returns(currentTime);
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<int>(new ConcurrencyException("Test concurrency exception", new Exception("Inner exception"))));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AlquilerErrors.ConcurrencyError);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateAlquilerWithCorrectProperties()
    {
        // Arrange
        var command = CreateValidCommand();
        var user = CreateTestUser();
        var vehiculo = CreateTestVehiculo();
        var currentTime = DateTime.UtcNow;

        _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _vehiculoRepository.GetByIdAsync(Arg.Any<VehiculoId>(), Arg.Any<CancellationToken>())
            .Returns(vehiculo);
        _alquilerRepository.IsOverLappingAsync(Arg.Any<Vehiculo>(), Arg.Any<DateRange>(), Arg.Any<CancellationToken>())
            .Returns(false);
        _dateTimeProvider.CurrenTime.Returns(currentTime);
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _alquilerRepository.Received(1).Add(Arg.Is<Alquiler>(alquiler =>
            alquiler.VehiculoId == vehiculo.Id &&
            alquiler.UserId == user.Id &&
            alquiler.Status == AlquilerStatus.Reservado &&
            alquiler.FechaCreacion == currentTime &&
            alquiler.PrecioPorPeriodo != null &&
            alquiler.PrecioMantenimiento != null &&
            alquiler.PrecioAccesorios != null &&
            alquiler.PrecioTotal != null));
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateVehiculoFechaUltimoAlquiler()
    {
        // Arrange
        var command = CreateValidCommand();
        var user = CreateTestUser();
        var vehiculo = CreateTestVehiculo();
        var currentTime = DateTime.UtcNow;

        _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _vehiculoRepository.GetByIdAsync(Arg.Any<VehiculoId>(), Arg.Any<CancellationToken>())
            .Returns(vehiculo);
        _alquilerRepository.IsOverLappingAsync(Arg.Any<Vehiculo>(), Arg.Any<DateRange>(), Arg.Any<CancellationToken>())
            .Returns(false);
        _dateTimeProvider.CurrenTime.Returns(currentTime);
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        vehiculo.FechaUltimoAlquiler.Should().NotBeNull();
        vehiculo.FechaUltimoAlquiler.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldRaiseDomainEvent()
    {
        // Arrange
        var command = CreateValidCommand();
        var user = CreateTestUser();
        var vehiculo = CreateTestVehiculo();
        var currentTime = DateTime.UtcNow;

        _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _vehiculoRepository.GetByIdAsync(Arg.Any<VehiculoId>(), Arg.Any<CancellationToken>())
            .Returns(vehiculo);
        _alquilerRepository.IsOverLappingAsync(Arg.Any<Vehiculo>(), Arg.Any<DateRange>(), Arg.Any<CancellationToken>())
            .Returns(false);
        _dateTimeProvider.CurrenTime.Returns(currentTime);
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _alquilerRepository.Received(1).Add(Arg.Is<Alquiler>(alquiler =>
            alquiler.GetDomainEvents().Any(e => e is AlquilerReservadoDomainEvent)));
    }

    [Theory]
    [InlineData(1, 100, 20, 12, 132)] // 1 día, precio 100, mantenimiento 20, accesorios 12
    [InlineData(3, 100, 20, 18, 338)] // 3 días, precio 100, mantenimiento 20, accesorios 18
    [InlineData(7, 100, 20, 42, 762)] // 7 días, precio 100, mantenimiento 20, accesorios 42
    public async Task Handle_WithDifferentPeriods_ShouldCalculateCorrectPrecios(int dias, decimal precioBase, decimal mantenimiento, decimal accesorios, decimal totalEsperado)
    {
        // Arrange
        var command = new ReservarAlquilerCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(dias)));

        var user = CreateTestUser();
        var vehiculo = CreateTestVehiculoWithCustomPrecio(precioBase, mantenimiento);
        var currentTime = DateTime.UtcNow;

        _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _vehiculoRepository.GetByIdAsync(Arg.Any<VehiculoId>(), Arg.Any<CancellationToken>())
            .Returns(vehiculo);
        _alquilerRepository.IsOverLappingAsync(Arg.Any<Vehiculo>(), Arg.Any<DateRange>(), Arg.Any<CancellationToken>())
            .Returns(false);
        _dateTimeProvider.CurrenTime.Returns(currentTime);
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _alquilerRepository.Received(1).Add(Arg.Is<Alquiler>(alquiler =>
            alquiler.PrecioPorPeriodo!.Monto == precioBase * dias &&
            alquiler.PrecioMantenimiento!.Monto == mantenimiento &&
            alquiler.PrecioAccesorios!.Monto == accesorios &&
            alquiler.PrecioTotal!.Monto == totalEsperado));
    }

    private static ReservarAlquilerCommand CreateValidCommand()
    {
        return new ReservarAlquilerCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)));
    }

    private static User CreateTestUser()
    {
        return User.Create(
            new Nombre("John"),
            new Apellido("Doe"),
            new Email("john.doe@example.com"),
            new PasswordHash("hashedPassword123"));
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
            new Direccion("123 Main St", "New York", "NY", "Manhattan", "USA"));
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
            new Direccion("123 Main St", "New York", "NY", "Manhattan", "USA"));
    }
}

