using CleanArchitecture.Application.Abstractions.Messaging;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Vehiculos;
using CleanArchitecture.Domain.Vehiculos.Specifications;

namespace CleanArchitecture.Application.Vehiculos.GetVehiculosByPagination;

internal sealed class GetVehiculosByPaginationQueryHandler
: IQueryHandler<GetVehiculosByPaginationQuery, PaginationResult<Vehiculo, VehiculoId>>
{
    private readonly IVehiculoRepository _vehiculoRepository;

    public GetVehiculosByPaginationQueryHandler(IVehiculoRepository vehiculoRepository)
    {
        _vehiculoRepository = vehiculoRepository;
    }

    public async Task<Result<PaginationResult<Vehiculo, VehiculoId>>> Handle(
        GetVehiculosByPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new VehiculoPaginationSpecification(
            request.Sort!,
            request.PageIndex,
            request.PageSize,
            request.Search!);

        var vehiculos = await _vehiculoRepository.GetAllWithSpec(spec, cancellationToken);

        var countSpec = new VehiculoPaginationCountingSpecification(request.Search!);

        var totalRecords = await _vehiculoRepository.CountAsync(countSpec, cancellationToken);

        var totalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords) / Convert.ToDecimal(request.PageSize)));

        var recordsByPage = vehiculos.Count;

        return new PaginationResult<Vehiculo, VehiculoId>
        {
            Count = totalRecords,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            ResultByPage = recordsByPage,
            Data = vehiculos
        };
    }
}