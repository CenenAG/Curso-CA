using System.Reflection;
using CleanArchitecture.ArchitectureTests.Infrastructure;
using CleanArchitecture.Domain.Abstractions;
using NetArchTest.Rules;
using Xunit;

namespace CleanArchitecture.ArchitectureTests.Domain;

public class DomainTest : BaseTest
{
    [Fact]
    public void Entities_ShouldHave_PrivateConstructorNoParameters()
    {
        IEnumerable<Type> entityTypes = Types.InAssembly(DomainAssembly)
        .That()
        .Inherit(typeof(Entity<>))
        .GetTypes();

        var errorEntities = new List<Type>();

        foreach (Type entityType in entityTypes)
        {
            ConstructorInfo[] constructores = entityType.GetConstructors(
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            if (!constructores.Any(c => c.IsPrivate && c.GetParameters().Length == 0))
            {
                errorEntities.Add(entityType);
            }
        }

        //imprimir en consola errorEntities
        foreach (Type entityType in errorEntities)
        {
            Console.WriteLine("Error en la entidad : " + entityType.Name);
        }

        Assert.Empty(errorEntities);
    }
}