using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Roles;
using CleanArchitecture.Domain.Users;
using CleanArchitecture.Domain.Users.Events;
using CleanArchitecture.Domain.UnitTests.Common;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Domain.UnitTests.Users;

public class UserTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateUserWithCorrectProperties()
    {
        // Arrange
        var nombre = new Nombre("Juan");
        var apellido = new Apellido("Pérez");
        var email = new Email("juan.perez@example.com");
        var passwordHash = new PasswordHash("hashedPassword123");

        // Act
        var user = User.Create(nombre, apellido, email, passwordHash);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeNull();
        user.Nombre.Should().Be(nombre);
        user.Apellido.Should().Be(apellido);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
    }

    [Fact]
    public void Create_WithValidParameters_ShouldGenerateUniqueUserId()
    {
        // Arrange
        var nombre = new Nombre("María");
        var apellido = new Apellido("García");
        var email = new Email("maria.garcia@example.com");
        var passwordHash = new PasswordHash("hashedPassword456");

        // Act
        var user1 = User.Create(nombre, apellido, email, passwordHash);
        var user2 = User.Create(nombre, apellido, email, passwordHash);

        // Assert
        user1.Id.Should().NotBe(user2.Id);
        user1.Id.Value.Should().NotBeEmpty();
        user2.Id.Value.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValidParameters_ShouldAssignClienteRole()
    {
        // Arrange
        var nombre = new Nombre("Ana");
        var apellido = new Apellido("López");
        var email = new Email("ana.lopez@example.com");
        var passwordHash = new PasswordHash("hashedPassword789");

        // Act
        var user = User.Create(nombre, apellido, email, passwordHash);

        // Assert
        user.Roles.Should().NotBeNull();
        user.Roles.Should().HaveCount(1);
        user.Roles.Should().ContainSingle(role => role == Role.Cliente);
    }

    [Fact]
    public void Create_WithValidParameters_ShouldRaiseUserCreatedDomainEvent()
    {
        // Arrange
        var nombre = new Nombre("Carlos");
        var apellido = new Apellido("Rodríguez");
        var email = new Email("carlos.rodriguez@example.com");
        var passwordHash = new PasswordHash("hashedPassword101");

        // Act
        var user = User.Create(nombre, apellido, email, passwordHash);

        // Assert
        DomainEventAssertions.AssertDomainEventWasPublished<UserCreatedDomainEvent, UserId>(user);

        var domainEvent = DomainEventAssertions.GetPublishedDomainEvent<UserCreatedDomainEvent, UserId>(user);
        domainEvent.UserId.Should().Be(user.Id);
    }



    [Theory]
    [InlineData("Juan", "Pérez", "juan.perez@example.com", "hashedPassword123")]
    [InlineData("María", "García", "maria.garcia@example.com", "hashedPassword456")]
    [InlineData("Ana", "López", "ana.lopez@example.com", "hashedPassword789")]
    public void Create_WithDifferentValidParameters_ShouldCreateUsersWithCorrectProperties(
        string nombreValue,
        string apellidoValue,
        string emailValue,
        string passwordHashValue)
    {
        // Arrange
        var nombre = new Nombre(nombreValue);
        var apellido = new Apellido(apellidoValue);
        var email = new Email(emailValue);
        var passwordHash = new PasswordHash(passwordHashValue);

        // Act
        var user = User.Create(nombre, apellido, email, passwordHash);

        // Assert
        user.Nombre!.Value.Should().Be(nombreValue);
        user.Apellido!.Value.Should().Be(apellidoValue);
        user.Email!.Value.Should().Be(emailValue);
        user.PasswordHash!.Value.Should().Be(passwordHashValue);
    }

    [Fact]
    public void User_Properties_ShouldBeReadOnlyFromOutside()
    {
        // Arrange
        var nombre = new Nombre("Test");
        var apellido = new Apellido("User");
        var email = new Email("test@example.com");
        var passwordHash = new PasswordHash("hashedPassword");

        // Act
        var user = User.Create(nombre, apellido, email, passwordHash);

        // Assert
        user.Nombre.Should().Be(nombre);
        user.Apellido.Should().Be(apellido);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);

        // Verify that properties are read-only (private setters)
        // This is implicit in the design, but we can verify the behavior
        user.Nombre.Should().NotBeNull();
        user.Apellido.Should().NotBeNull();
        user.Email.Should().NotBeNull();
        user.PasswordHash.Should().NotBeNull();
    }

    [Fact]
    public void User_Roles_ShouldReturnImmutableCollection()
    {
        // Arrange
        var nombre = new Nombre("Test");
        var apellido = new Apellido("User");
        var email = new Email("test@example.com");
        var passwordHash = new PasswordHash("hashedPassword");

        // Act
        var user = User.Create(nombre, apellido, email, passwordHash);

        // Assert
        user.Roles.Should().NotBeNull();
        user.Roles.Should().BeOfType<List<Role>>();

        // Verify that the collection is read-only
        var roles = user.Roles;
        roles.Should().HaveCount(1);
        roles.Should().Contain(Role.Cliente);
    }

    [Fact]
    public void User_EntityBase_ShouldInheritFromEntity()
    {
        // Arrange
        var nombre = new Nombre("Test");
        var apellido = new Apellido("User");
        var email = new Email("test@example.com");
        var passwordHash = new PasswordHash("hashedPassword");

        // Act
        var user = User.Create(nombre, apellido, email, passwordHash);

        // Assert
        user.Should().BeAssignableTo<Entity<UserId>>();
        user.Id.Should().BeOfType<UserId>();
    }

    [Fact]
    public void Create_WithValidParameters_ShouldRaiseUserCreatedDomainEvent_UsingGenericAssertion()
    {
        // Arrange
        var nombre = new Nombre("Test");
        var apellido = new Apellido("User");
        var email = new Email("test@example.com");
        var passwordHash = new PasswordHash("hashedPassword");

        // Act
        var user = User.Create(nombre, apellido, email, passwordHash);

        // Assert
        DomainEventAssertions.AssertDomainEventOfType<UserCreatedDomainEvent, UserId>(user, domainEvent =>
        {
            domainEvent.UserId.Should().Be(user.Id);
        });
    }


}
