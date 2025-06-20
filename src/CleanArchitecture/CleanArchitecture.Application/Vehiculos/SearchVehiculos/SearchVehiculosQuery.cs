using CleanArchitecture.Application.Abstractions.Messaging;

namespace CleanArchitecture.Application.Vehiculos.SearchVehiculos;

public record SearchVehiculosQuery(
    DateOnly FechaInicio,
    DateOnly FechaFin)
    : IQuery<IReadOnlyList<VehiculoResponse>>;