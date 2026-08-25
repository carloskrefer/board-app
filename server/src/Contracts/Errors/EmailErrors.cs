using Contracts.Errors;

public static class EmailResponseErrors
{
    public static BaseError Format => 
        new($"FORMAT","Invalid email address format. Expected: user@domain.com.");

    public static BaseError AlreadyRegistered => 
        new($"ALREADY_REGISTERED","Email is already registered.");
}