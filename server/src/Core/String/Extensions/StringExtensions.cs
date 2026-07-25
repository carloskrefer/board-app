namespace Core.String.Extensions;

public static class StringExtensions
{
    public static string FirstCharToLower(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        if (text.Length == 1)
            return text.ToLowerInvariant();

        return $"{char.ToLowerInvariant(text[0])}{text[1..]}";
    }

    public static string FromPascalCaseToCamelCase(this string text) => FirstCharToLower(text);

    public static bool HasAtLeastOneSpecialCharacter(this string text) => 
        text.Any(c => char.IsSymbol(c) || char.IsPunctuation(c));

    public static bool HasAtLeastOneCharacter(this string text) => text.Any(char.IsLetter);
    public static bool HasAtLeastOneUpperAndLowerCaseLetter(this string text) => 
        text.Any(char.IsUpper) && text.Any(char.IsLower);

    public static bool HasAtLeastOneNumber(this string text) => text.Any(char.IsNumber);
}