using Contracts.Errors;

public static class EmailResponseErrors
{
    private const string Id = "EMAIL";

    public static ApiError Format => 
        new($"{Id}.FORMAT","Invalid email address format. Expected: user@domain.com.");

    public static ApiError AlreadyRegistered => 
        new($"{Id}.ALREADY_REGISTERED","Email is already registered.");
}