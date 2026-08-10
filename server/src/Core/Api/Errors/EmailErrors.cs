using Core.Api.Errors;

public static class EmailResponseErrors
{
    public static GeneralResponseError Format => 
        new($"FORMAT","Invalid email address format. Expected: user@domain.com.");

}