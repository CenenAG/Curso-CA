using CleanArchitecture.Application.Abstractions.Messaging;
using CleanArchitecture.Application.Paginations;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Vehiculos;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Vehiculos.GetVehiculosGenPagination;




public class GetVehiculosGenPaginationQueryHandler
: IQueryHandler<GetVehiculosGenPaginationQuery, PagedResults<Vehiculo, VehiculoId>>
{
    private readonly IPaginationRepository<Vehiculo, VehiculoId> _paginationRepository;

    public GetVehiculosGenPaginationQueryHandler(IPaginationRepository<Vehiculo, VehiculoId> paginationRepository)
    {
        _paginationRepository = paginationRepository;
    }


    public async Task<Result<PagedResults<Vehiculo, VehiculoId>>> Handle(GetVehiculosGenPaginationQuery request, CancellationToken cancellationToken)
    {
        var predicateb = PredicateBuilder.New<Vehiculo>(true);

        if (!string.IsNullOrEmpty(request.Search))
        {
            predicateb = predicateb.Or(x => x.Modelo == new Modelo(request.Search));
            predicateb = predicateb.Or(x => x.Vin == new Vin(request.Search));
        }

        var pagedResultVehiculos = await _paginationRepository.GetPaginationAsync(
            predicate: predicateb,
            includes: p => p.Include(x => x.Direccion!),
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            orderBy: request.OrderBy!,
            isAscending: request.OrdesAsc,
            cancellationToken: cancellationToken);

        return pagedResultVehiculos;
    }
}