using System.Security.Cryptography.X509Certificates;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Roles;

namespace CleanArchitecture.Domain.Users;

public class User : Entity<UserId>
{
    private User()
    {
    }
    private readonly List<Role> _roles = new();

    private User(
        UserId id,
        Nombre? nombre = null,
        Apellido? apellido = null,
        Email? email = null,
        PasswordHash? passwordHash = null
    ) : base(id)
    {
        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        PasswordHash = passwordHash;
    }
    public Nombre? Nombre { get; private set; }
    public Apellido? Apellido { get; private set; }
    public Email? Email { get; private set; }
    public PasswordHash? PasswordHash { get; private set; }
    public IReadOnlyCollection<Role>? Roles => _roles.ToList();

    public static User Create(
        Nombre nombre,
        Apellido apellido,
        Email email,
        PasswordHash passwordHash
    )
    {
        var user = new User(
            UserId.New(),
            nombre,
            apellido,
            email,
            passwordHash
        );

        // Raise domain event
        user.RaiseDomainEvent(new Events.UserCreatedDomainEvent(user.Id));
        user._roles.Add(Role.Cliente);

        return user;
    }
}
