using CleanArchitecture.Application.Abstractions.Messaging;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Shared;
using CleanArchitecture.Domain.Vehiculos;

namespace CleanArchitecture.Application.Vehiculos.GetVehiculosGenPagination;

public record GetVehiculosGenPaginationQuery
: PaginationParams, IQuery<PagedResults<Vehiculo, VehiculoId>>
{

}