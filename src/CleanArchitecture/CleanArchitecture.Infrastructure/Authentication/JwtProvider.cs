using CleanArchitecture.Application.Abstractions.Data;
using CleanArchitecture.Application.Authentication;
using CleanArchitecture.Domain.Users;
using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CleanArchitecture.Infrastructure.Authentication;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public JwtProvider(IOptions<JwtOptions> jwtOptions, ISqlConnectionFactory sqlConnectionFactory)
    {
        _jwtOptions = jwtOptions.Value;
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        const string sql = """"
                        SELECT
                            distinct p.nombre
                        FROM users usr
                            LEFT JOIN user_roles usrl
                                ON usr.id = usrl.user_id
                            LEFT JOIN roles rl
                                ON usrl.role_id = rl.id
                            LEFT JOIN rol_permissions rp
                                ON rl.id = rp.role_id
                            LEFT JOIN permissions p
                                ON rp.permission_id = p.id
                        WHERE usr.id = @UserId
                        """";

        using var connection = _sqlConnectionFactory.CreateConnection();

        var permissions =
        await connection.QueryAsync<string>(sql, new { UserId = user.Id!.Value });

        var permissionCollection = permissions.ToHashSet();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!.Value),
        };

        foreach (var permission in permissionCollection)
        {
            claims.Add(new Claim(CustomClaims.Permissions, permission));
        }

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: null,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            signingCredentials: signingCredentials
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return await Task.FromResult(tokenValue);
    }
}