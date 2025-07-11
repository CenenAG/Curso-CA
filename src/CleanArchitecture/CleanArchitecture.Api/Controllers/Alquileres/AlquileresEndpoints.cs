using Asp.Versioning;
using Asp.Versioning.Builder;
using CleanArchitecture.Api.Utils;
using CleanArchitecture.Application.Alquileres.GetAlquiler;
using CleanArchitecture.Application.Alquileres.ReservarAlquiler;
using CleanArchitecture.Domain.Permissions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers.Alquileres;

public static class AlquileresEndpoints
{

    public static IEndpointRouteBuilder MapAlquilerEndPoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("alquileres/{id}", GetAlquiler)
        .RequireAuthorization(PermissionEnum.ReadUser.ToString())
        .WithName(nameof(GetAlquiler));

        builder
        .MapPost("alquileres", ReservarAlquiler)
            .Produces<Guid>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionEnum.WriteUser.ToString());

        return builder;
    }


    public static async Task<IResult> GetAlquiler(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetAlquilerQuery(id);
        var alquiler = await sender.Send(query, cancellationToken);
        return alquiler.IsSuccess ? Results.Ok(alquiler.Value) : Results.NotFound();
    }

    public static async Task<IResult> ReservarAlquiler(
        ISender sender,
        AlquilerReservaRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ReservarAlquilerCommand(
            request.VehiculoId,
            request.UserId,
            request.StartDate,
            request.EndDate);

        var resultado = await sender.Send(command, cancellationToken);
        if (resultado.IsSuccess == false) return Results.BadRequest(resultado.Error);

        // Devuelve el GUID en el cuerpo y la URL de ubicación en el header
        return Results.CreatedAtRoute(nameof(GetAlquiler), new { id = resultado.Value }, resultado.Value);
    }
}
