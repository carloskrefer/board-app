namespace Core.Api.Errors;

public static class TextResponseErrors
{
    public static GeneralResponseError Empty(string textType = "Text") => 
        new(
            $"EMPTY", 
            $"{textType} is empty.");

    public static GeneralResponseError TooLong(int maxLength, string textType = "Text") => 
        new(
            $"TOO_LONG", 
            $"{textType} should have less than {maxLength} characters.");

    public static GeneralResponseError BetweenRange(int minLength, int maxLength, string textType = "Text") => 
        new(
            $"BETWEEN_RANGE", 
            $"{textType} should have between {minLength} and {maxLength} characters.");

    public static GeneralResponseError OnlySpecialCharacters(string textType = "Text") => 
        new(
            $"ONLY_SPECIAL_CHARACTERS", 
            $"{textType} should not only have special characters.");
}