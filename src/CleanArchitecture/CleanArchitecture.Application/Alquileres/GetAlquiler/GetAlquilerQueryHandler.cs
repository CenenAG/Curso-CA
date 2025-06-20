using System.Data;
using CleanArchitecture.Application.Abstractions.Data;
using CleanArchitecture.Application.Abstractions.Messaging;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Alquileres;
using Dapper;
using MediatR;

namespace CleanArchitecture.Application.Alquileres.GetAlquiler;

internal sealed class GetAlquilerQueryHandler : IQueryHandler<GetAlquilerQuery, AlquilerResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetAlquilerQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<AlquilerResponse>> Handle(
        GetAlquilerQuery request,
        CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = """
                SELECT id as Id, 
                vehiculo_Id as VehiculoId, 
                user_id as UserId,
                status as Status,
                precio_por_periodo_monto as PrecioAlquiler,
                precio_por_periodo_tipo_moneda  as TipoMonedaAlquiler,
                mantenimiento_monto as PrecioMantenimiento,
                mantenimiento_tipo_moneda as TipoMonedaMantenimiento,
                accesorios_monto as PrecioAccesorios,
                accesorios_tipo_moneda as TipoMonedaAccesorios,
                precio_total_monto as PrecioTotal,
                precio_total_tipo_moneda as TipoMonedaPrecioTotal,
                duracion_inicio as DuracionInicio,
                duracion_fin as DuracionFinal,
                fecha_creacion as FechaCreacion

                FROM Alquileres 
                WHERE Id = @AlquilerId
                """;

        var alquiler = await connection.QueryFirstOrDefaultAsync<AlquilerResponse>(
            sql,
            new { request.AlquilerId });

        return alquiler!;
    }


}
