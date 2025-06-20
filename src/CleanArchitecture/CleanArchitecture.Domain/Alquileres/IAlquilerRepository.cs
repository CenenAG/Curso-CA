using CleanArchitecture.Domain.Vehiculos;

namespace CleanArchitecture.Domain.Alquileres;

public interface IAlquilerRepository
{
    public Task<Alquiler?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    public Task<bool?> IsOverLappingAsync(
        Vehiculo vehiculo,
        DateRange dateRange,
        CancellationToken cancellationToken = default);
    void Add(Alquiler alquiler);
}