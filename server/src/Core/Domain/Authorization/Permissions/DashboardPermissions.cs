namespace Core.Domain.Authorization.Permissions;

// TODO: Will not use it now, should be inside the new module (each module will define its own roles and permissions)
public static class DashboardPermissions
{
    private const string _prefix = "dashboard";
    public const string PageView = $"{_prefix}.page.view";
}