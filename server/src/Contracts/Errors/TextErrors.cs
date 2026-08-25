namespace Contracts.Errors;

public static class TextResponseErrors
{
    public static BaseError Empty(string textType = "Text") => 
        new(
            $"EMPTY", 
            $"{textType} is empty.");

    public static BaseError TooLong(int maxLength, string textType = "Text") => 
        new(
            $"TOO_LONG", 
            $"{textType} should have less than {maxLength} characters.");

    public static BaseError BetweenRange(int minLength, int maxLength, string textType = "Text") => 
        new(
            $"BETWEEN_RANGE", 
            $"{textType} should have between {minLength} and {maxLength} characters.");

    public static BaseError OnlySpecialCharacters(string textType = "Text") => 
        new(
            $"ONLY_SPECIAL_CHARACTERS", 
            $"{textType} should not only have special characters.");
}