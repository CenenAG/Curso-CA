using CleanArchitecture.Domain.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(role => role.Id);

        // Configurar los valores de enumeración
        builder.HasData(Role.GetValues());

        builder.HasMany(role => role.Permissions)
            .WithMany()
            .UsingEntity<RolPermission>();
    }
}
