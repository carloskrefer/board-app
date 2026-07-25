using System.Text.RegularExpressions;

namespace Auth.Domain.ValueObjects;

public sealed record Name(string Value)
{
    public static Result<Name> Create(string value)
    {
        var valueCleaned = Regex.Replace(value.Trim(), @"\s+", " ");

        if (string.IsNullOrWhiteSpace(valueCleaned))
            return NameErrors.Empty;

        if (valueCleaned.Length > 100)
            return NameErrors.TooLong;

        if (!Regex.IsMatch(valueCleaned, @"[a-zA-Z0-9]"))
            return NameErrors.OnlySpecialCharacters;

        return new Name(valueCleaned);
    }
}

public static class NameErrors
{
    public static readonly string Id = "NAME";
    public static readonly Error Empty = 
        new(
            $"{Id}.EMPTY", 
            "Name is empty.");
    public static readonly Error TooLong = 
        new(
            $"{Id}.TOO_LONG", 
            "Name should have less than 100 characters.");

    public static readonly Error OnlySpecialCharacters = 
        new(
            $"{Id}.ONLY_SPECIAL", 
            "Name should not only have special characters.");
}