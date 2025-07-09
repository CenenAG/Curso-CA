using Asp.Versioning;
using CleanArchitecture.Api.Utils;
using CleanArchitecture.Application.Alquileres.GetAlquiler;
using CleanArchitecture.Application.Alquileres.ReservarAlquiler;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers.Alquileres;

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/alquileres")]
public class AlquileresController : ControllerBase
{
    private readonly ISender _sender;

    public AlquileresController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlquiler(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetAlquilerQuery(id);
        var alquiler = await _sender.Send(query, cancellationToken);
        return alquiler.IsSuccess ? Ok(alquiler.Value) : NotFound();
    }

    [HttpPost("reservar/{vehiculoId}")]
    public async Task<IActionResult> ReservarAlquiler(
        Guid id,
        AlquilerReservaRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ReservarAlquilerCommand(
            request.VehiculoId,
            request.UserId,
            request.StartDate,
            request.EndDate);

        var resultado = await _sender.Send(command, cancellationToken);
        if (resultado.IsSuccess == false) return BadRequest(resultado.Error);
        return CreatedAtAction(nameof(GetAlquiler), new { id = resultado.Value }, null);
    }
}
