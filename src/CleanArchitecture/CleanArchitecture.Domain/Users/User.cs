using System.Security.Cryptography.X509Certificates;
using CleanArchitecture.Domain.Abstractions;

namespace CleanArchitecture.Domain.Users;

public class User : Entity<UserId>
{
    public User()
    {

    }
    private User(
        UserId id,
        Nombre? nombre = null,
        Apellido? apellido = null,
        Email? email = null
    ) : base(id)
    {
        Nombre = nombre;
        Apellido = apellido;
        Email = email;
    }
    public Nombre? Nombre { get; private set; }
    public Apellido? Apellido { get; private set; }
    public Email? Email { get; private set; }

    public static User Create(
        Nombre nombre,
        Apellido apellido,
        Email email)
    {
        var user = new User(
            UserId.New(),
            nombre,
            apellido,
            email
        );

        // Raise domain event
        user.RaiseDomainEvent(new Events.UserCreatedDomainEvent(user.Id));

        return user;
    }
}
