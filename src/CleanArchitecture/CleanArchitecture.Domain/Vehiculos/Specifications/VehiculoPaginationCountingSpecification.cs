using System.Linq.Expressions;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Shared;

namespace CleanArchitecture.Domain.Vehiculos.Specifications;

public class VehiculoPaginationCountingSpecification : BaseSpecification<Vehiculo, VehiculoId>
{


    public VehiculoPaginationCountingSpecification(string search)
    : base(x => x.Modelo == new Modelo(search) || string.IsNullOrEmpty(search))
    {

    }


}

