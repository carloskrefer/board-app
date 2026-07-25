using Core.Api.Errors;

public static class EmailResponseErrors
{    
    public static readonly string Id = "EMAIL";
    public static GeneralResponseError Format => 
        new($"{Id}.FORMAT","Invalid email address format. Expected: user@domain.com.");
}