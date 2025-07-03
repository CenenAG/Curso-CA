using CleanArchitecture.Domain.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");
        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Id)
            .HasConversion(id => id!.Value, value => new PermissionId(value));

        builder.Property(permission => permission.Nombre)
            .HasConversion(nombre => nombre!.Value, value => new Nombre(value));

        IEnumerable<Permission> permissions = Enum.GetValues<PermissionEnum>()
            .Select(p => new Permission(
                new PermissionId((int)p),
                new Nombre(p.ToString())
                ));

        builder.HasData(permissions);
    }
}