using CleanArchitecture.Domain.Abstractions;

namespace CleanArchitecture.Domain.Vehiculos;

public interface IVehiculoRepository
{
    Task<Vehiculo?> GetByIdAsync(VehiculoId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Vehiculo>> GetAllWithSpec(
        ISpecification<Vehiculo, VehiculoId> spec,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        ISpecification<Vehiculo, VehiculoId> spec,
        CancellationToken cancellationToken = default);
}
