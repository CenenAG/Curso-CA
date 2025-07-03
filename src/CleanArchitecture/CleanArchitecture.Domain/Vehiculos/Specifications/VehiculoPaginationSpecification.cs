using System.Linq.Expressions;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Shared;

namespace CleanArchitecture.Domain.Vehiculos.Specifications;

public class VehiculoPaginationSpecification : BaseSpecification<Vehiculo, VehiculoId>
{


    public VehiculoPaginationSpecification(
        string sort,
        int pageIndex,
        int pageSize,
        string search)
    : base(x => x.Modelo == new Modelo(search) || string.IsNullOrEmpty(search))
    {
        ApplyPaging(pageSize * (pageIndex - 1), pageSize);

        if (!string.IsNullOrEmpty(sort))
        {
            switch (sort)
            {
                case "modeloAsc":
                    AddOrderBy(p => p.Modelo!);
                    break;
                case "modeloDesc":
                    AddOrderByDescending(p => p.Modelo!);
                    break;
                default:
                    AddOrderBy(p => p.FechaUltimoAlquiler!);
                    break;
            }
        }
        else
        {
            AddOrderBy(p => p.FechaUltimoAlquiler!);
        }
    }


}

