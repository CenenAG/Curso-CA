using CleanArchitecture.Domain.Permissions;
using CleanArchitecture.Domain.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Configurations;

public sealed class RolPermissionConfiguration : IEntityTypeConfiguration<RolPermission>
{
    public void Configure(EntityTypeBuilder<RolPermission> builder)
    {
        builder.ToTable("rol_permissions");
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.Property(rp => rp.PermissionId)
            .HasConversion(permissionId => permissionId!.Value, value => new PermissionId(value));

        builder.HasData(
            Create(Role.Admin, PermissionEnum.ReadUser),
            Create(Role.Admin, PermissionEnum.WriteUser),
            Create(Role.Admin, PermissionEnum.UpdateUser),
            Create(Role.Cliente, PermissionEnum.ReadUser)
        );
    }


    private static RolPermission Create(Role role, PermissionEnum permission)
    {
        return new RolPermission
        {
            RoleId = role.Id,
            PermissionId = new PermissionId((int)permission)
        };
    }


}