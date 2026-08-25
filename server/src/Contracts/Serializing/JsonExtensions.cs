namespace Constracts.Serializing;

public static class JsonExtensions
{
    public static string ToCamelCase(this string pascalCase)
    {
        if (string.IsNullOrEmpty(pascalCase))
            return pascalCase;

        if (pascalCase.Length == 1)
            return pascalCase.ToLowerInvariant();

        return $"{char.ToLowerInvariant(pascalCase[0])}{pascalCase[1..]}";
    }
}