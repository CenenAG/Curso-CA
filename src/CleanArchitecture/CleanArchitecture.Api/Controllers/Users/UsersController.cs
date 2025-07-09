using Asp.Versioning;
using CleanArchitecture.Api.Utils;
using CleanArchitecture.Application.Users.GetUsersDapperPagination;
using CleanArchitecture.Application.Users.GetUsersPagination;
using CleanArchitecture.Application.Users.LoginUser;
using CleanArchitecture.Application.Users.RegisterUser;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers.Users;

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<IActionResult> LoginV1(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(result.Error);
        }

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.Nombre,
            request.Apellido);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(result.Error);
        }

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpGet("getPagination", Name = "PaginationUser")]
    [ProducesResponseType(typeof(PagedResults<User, UserId>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResults<User, UserId>>> GetPagination(
            [FromQuery] GetUsersPaginationQuery query,
            CancellationToken cancellationToken)
    {
        var resultados = await _sender.Send(query, cancellationToken);

        if (resultados.IsFailure)
        {
            return BadRequest(resultados.Error);
        }

        return Ok(resultados);
    }

    [AllowAnonymous]
    [HttpGet("getPaginationDapper", Name = "PaginationUserDapper")]
    [ProducesResponseType(typeof(PagedDapperResults<UserPaginationDapperData>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedDapperResults<UserPaginationDapperData>>> GetPaginationDapper(
        [FromQuery] GetUsersDapperPaginationQuery query,
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