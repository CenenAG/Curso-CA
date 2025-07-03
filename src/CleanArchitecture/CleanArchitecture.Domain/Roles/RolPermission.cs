using CleanArchitecture.Domain.Permissions;

namespace CleanArchitecture.Domain.Roles;

public sealed class RolPermission
{
    public int RoleId { get; set; }
    public PermissionId? PermissionId { get; set; }
}