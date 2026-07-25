namespace Core.Exceptions;

public class ConfigurationNotFound : Exception
{
    public ConfigurationNotFound(string configurationKey)
        : base($"Configuration key '{configurationKey}' not found.")
    {
    }
}