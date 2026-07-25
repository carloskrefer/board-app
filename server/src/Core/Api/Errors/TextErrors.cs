namespace Core.Api.Errors;

public static class TextResponseErrors
{    
    public static readonly string Id = "TEXT";
    public static GeneralResponseError Empty = 
        new(
            $"{Id}.EMPTY", 
            "Name is empty.");

    public static GeneralResponseError TooLong(int maxLength) => 
        new(
            $"{Id}.TOO_LONG", 
            $"Name should have less than {maxLength} characters.");

    public static GeneralResponseError OnlySpecialCharacters = 
        new(
            $"{Id}.ONLY_SPECIAL_CHARACTERS", 
            "Name should not only have special characters.");
}