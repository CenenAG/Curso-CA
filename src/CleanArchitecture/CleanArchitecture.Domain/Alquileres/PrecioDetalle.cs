using CleanArchitecture.Domain.Shared;
using CleanArchitecture.Domain.Vehiculos;

namespace CleanArchitecture.Domain.Alquileres;

public record PrecioDetalle(
    Moneda PrecioPorPeriodo,
    Moneda PrecioMantenimiento,
    Moneda PrecioAccesorios,
    Moneda PrecioTotal);