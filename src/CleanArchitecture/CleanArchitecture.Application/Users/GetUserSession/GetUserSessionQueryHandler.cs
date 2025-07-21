
using CleanArchitecture.Application.Abstractions.Authentication;
using CleanArchitecture.Application.Abstractions.Data;
using CleanArchitecture.Application.Abstractions.Messaging;
using CleanArchitecture.Domain.Abstractions;
using Dapper;

namespace CleanArchitecture.Application.Users.GetUserSession;

internal sealed class GetUserSessionQueryHandler
: IQueryHandler<GetUserSessionQuery, UserResponse>
{
    private readonly IUserContext _userContext;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;


    public GetUserSessionQueryHandler(IUserContext userContext, ISqlConnectionFactory sqlConnectionFactory)
    {
        _userContext = userContext;
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<UserResponse>> Handle(GetUserSessionQuery request, CancellationToken cancellationToken)
    {
        var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT 
                id as Id,       
                email as Email,         
                nombre as Nombre,
                apellido as Apellido               
            FROM users 
            WHERE email = @Email
            """;

        var user = await connection.QuerySingleOrDefaultAsync<UserResponse>(sql, new { Email = _userContext.UserEmail });


        return user!;
    }
}