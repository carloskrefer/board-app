namespace Core.Predicates;

public static class EmailPredicates
{
    public static bool IsEmailFormatValid(this string email) =>
        email.Contains("@")
            && email.IndexOf("@") > 0
            && email.IndexOf("@") < email.Length - 1;
}