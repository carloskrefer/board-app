using Core.Api.Errors;

public static class EmailResponseErrors
{
    public static GeneralResponseError Format => 
        new($"FORMAT","Invalid email address format. Expected: user@domain.com.");

    public static GeneralResponseError AlreadyRegistered => 
        new($"ALREADY_REGISTERED","Email is already registered.");
}