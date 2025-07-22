using Asp.Versioning;
using CleanArchitecture.Api.Utils;
using CleanArchitecture.Application.Vehiculos.GetVehiculosByPagination;
using CleanArchitecture.Application.Vehiculos.GetVehiculosGenPagination;
using CleanArchitecture.Application.Vehiculos.ReportVehiculoPdf;
using CleanArchitecture.Application.Vehiculos.SearchVehiculos;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Permissions;
using CleanArchitecture.Domain.Vehiculos;
using CleanArchitecture.Infrastructure.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

namespace CleanArchitecture.Api.Controllers.Vehiculos;


[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/vehiculos")]
public class VehiculosController : ControllerBase
{
    private readonly ISender _sender;

    public VehiculosController(ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpGet("report")]
    public async Task<IActionResult> GetReportVehiculos(
        CancellationToken cancellationToken,
        string modelo = "")
    {
        var query = new ReportVehiculoPdfQuery(modelo);
        var resultado = await _sender.Send(query, cancellationToken);
        byte[] pdfBytes = resultado.Value.GeneratePdf();
        return File(pdfBytes, "application/pdf");
    }

    [HasPermission(PermissionEnum.ReadUser)]
    [HttpGet("search")]
    public async Task<IActionResult> SearchVehiculos(
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken)
    {
        var query = new SearchVehiculosQuery(startDate, endDate);
        var resultados = await _sender.Send(query, cancellationToken);
        return Ok(resultados.Value);
    }

    [AllowAnonymous]
    [HttpGet("getPagination", Name = "PaginationVehiculos")]
    [ProducesResponseType(typeof(PaginationResult<Vehiculo, VehiculoId>), (int)StatusCodes.Status200OK)]
    public async Task<Result<PaginationResult<Vehiculo, VehiculoId>>> GetVehiculosByPagination(
        [FromQuery] GetVehiculosByPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var resultados = await _sender.Send(request, cancellationToken);
        return resultados;
    }

    [AllowAnonymous]
    [HttpGet("getGenPagination", Name = "genPaginationUser")]
    [ProducesResponseType(typeof(PagedResults<Vehiculo, VehiculoId>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResults<Vehiculo, VehiculoId>>> GetPagination(
           [FromQuery] GetVehiculosGenPaginationQuery query,
           CancellationToken cancellationToken)
    {
        var resultados = await _sender.Send(query, cancellationToken);

        if (resultados.IsFailure)
        {
            return BadRequest(resultados.Error);
        }

        return Ok(resultados);
    }
}
