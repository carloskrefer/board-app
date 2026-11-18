namespace Core.Api;

public static class ConcurrencyHelper
{
    public static string GenerateUpdateConflictDetail(string resourceName, string? resourceId = null)
    {
        var phrase1 = $"Resource '{resourceName}' ";
        var phrase2 = resourceId is null ? string.Empty : $"identified by '{resourceId}' ";
        var phrase3 = "has been concurrently modified by another request. Please reload and try again.";
        return $"{phrase1}{phrase2}{phrase3}";
    }
}