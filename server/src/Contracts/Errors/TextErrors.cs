namespace Contracts.Errors;

public static class TextResponseErrors
{
    public static ApiError Empty(string textType = "Text") => 
        new(
            $"EMPTY", 
            $"{textType} is empty.");

    public static ApiError TooLong(int maxLength, string textType = "Text") => 
        new(
            $"TOO_LONG", 
            $"{textType} should have less than {maxLength} characters.");

    public static ApiError TooSmall(int minLength, string textType = "Text") => 
        new(
            $"TOO_SMALL", 
            $"{textType} should have more than {minLength} characters.");

    // TODO: Remove, use either TooLong or TooSmall
    public static ApiError BetweenRange(int minLength, int maxLength, string textType = "Text") => 
        new(
            $"BETWEEN_RANGE", 
            $"{textType} should have between {minLength} and {maxLength} characters.");

    public static ApiError OnlySpecialCharacters(string textType = "Text") => 
        new(
            $"ONLY_SPECIAL_CHARACTERS", 
            $"{textType} should not only have special characters.");

    public static ApiError AtLeastOneSpecialCharacter(string textType = "Text") => 
        new(
            $"AT_LEAST_ONE_SPECIAL_CHARACTER", 
            $"{textType} should have at least one special character.");    


    public static ApiError AtLeastOneNumber(string textType = "Text") => 
        new(
            $"AT_LEAST_ONE_NUMBER", 
            $"{textType} should have at least one number.");

    public static ApiError AtLeastOneUpperAndLowerCase(string textType = "Text") => 
        new(
            $"AT_LEAST_ONE_UPPER_AND_LOWER_CASE", 
            $"{textType} should have at least one uppercase and one lowercase letter.");
}