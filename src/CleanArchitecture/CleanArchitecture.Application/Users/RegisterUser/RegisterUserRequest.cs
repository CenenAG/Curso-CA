using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.Users.RegisterUser;

public sealed record RegisterUserRequest(
        string Email,
        string Nombre,
        string Apellido,
        string Password
);
