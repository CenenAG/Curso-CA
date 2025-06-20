
using CleanArchitecture.Application.Abstractions.Messaging;

namespace CleanArchitecture.Application.Alquileres.ReservarAlquiler;

public sealed record ReservarAlquilerCommand(
    Guid VehiculoId,
    Guid UserId,
    DateOnly FechaInicio,
    DateOnly FechaFin
) : ICommand<Guid>;