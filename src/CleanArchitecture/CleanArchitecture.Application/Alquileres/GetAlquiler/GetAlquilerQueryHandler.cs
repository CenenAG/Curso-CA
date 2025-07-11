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
                 SELECT
                    id AS Id,
                    vehiculo_id AS VehiculoId,
                    user_id AS UserId,
                    status AS Status,
                    precio_por_periodo_monto AS PrecioAlquiler,
                    precio_por_periodo_tipo_moneda AS TipoMonedaAlquiler,
                    precio_mantenimiento_monto AS PrecioMantenimiento,
                    precio_mantenimiento_tipo_moneda AS TipoMonedaMantenimiento,
                    precio_accesorios_monto AS AccesoriosPrecio,
                    precio_accesorios_tipo_moneda AS TipoMonedaAccesorio,
                    precio_total_monto AS PrecioTotal,
                    precio_total_tipo_moneda AS PrecioTotalTipoMoneda,
                    duracion_inicio AS DuracionInicio,
                    duracion_fin AS DuracionFinal,
                    fecha_creacion AS FechaCreacion
                FROM alquileres WHERE id=@AlquilerId  
                """;

        var alquiler = await connection.QueryFirstOrDefaultAsync<AlquilerResponse>(
            sql,
            new { request.AlquilerId });

        return alquiler!;
    }


}
