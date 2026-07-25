using System.Collections.Frozen;
using Core.Domain.Authorization.Permissions;

namespace Core.Domain.Authorization;

// TODO: Will not use it now, should be inside the new module (each module will define its own roles and permissions)
public static class Roles
{
    public const string User = "user";

    public static readonly FrozenDictionary<string, FrozenSet<string>> Permissions =
        new Dictionary<string, FrozenSet<string>>
        {
            [User] = new[]
            {
                DashboardPermissions.PageView
            }.ToFrozenSet()
        }.ToFrozenDictionary();
}