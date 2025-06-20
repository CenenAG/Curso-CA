using CleanArchitecture.Application.Abstractions.Data;
using CleanArchitecture.Application.Abstractions.Messaging;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Alquileres;
using CleanArchitecture.Domain.Vehiculos;
using Dapper;
using MediatR;

namespace CleanArchitecture.Application.Vehiculos.SearchVehiculos;

internal sealed class SearchVehiculosQueryHandler : IQueryHandler<SearchVehiculosQuery, IReadOnlyList<VehiculoResponse>>
{
    private static readonly int[] ActiveAlquilerStatuses =
    {
        (int)AlquilerStatus.Reservado,
        (int)AlquilerStatus.Confirmado,
        (int)AlquilerStatus.Completado
    };

    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public SearchVehiculosQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IReadOnlyList<VehiculoResponse>>> Handle(
        SearchVehiculosQuery request,
        CancellationToken cancellationToken)
    {
        if (request.FechaInicio >= request.FechaFin)
        {
            return new List<VehiculoResponse>();
        }

        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = """
                SELECT 
                    a.id as Id,
                    a.modelo as Modelo,
                    a.vin as Vin,
                    a.precio_monto as Precio,
                    a.precio_tipo_moneda as TipoMoneda,
                    a.direccion_pais as Pais,
                    a.direccion_departamento as Departamento,
                    a.direccion_provincia as Provincia,
                    a.direccion_ciudad as Ciudad,
                    a.direccion_calle as Calle

                FROM Vehiculos  AS a
                WHERE NOT EXISTS(
                    SELECT 1
                    FROM Alquileres AS b
                    WHERE a.id = b.vehiculo_id AND
                    b.duracion_inicio <= @FechaFin AND 
                    b.duracion_fin >= @FechaInicio AND
                    b.status = ANY(@ActiveAlquilerStatuses)
                )

                """;

        var vehiculos = await connection
        .QueryAsync<VehiculoResponse, DireccionResponse, VehiculoResponse>
        (
            sql,
            (vehiculo, direccion) =>
            {
                vehiculo.Direccion = direccion;
                return vehiculo;
            },
            new
            {
                request.FechaInicio,
                request.FechaFin,
                ActiveAlquilerStatuses
            },
            splitOn: "Pais"
        );

        return vehiculos.ToList();
    }
}

