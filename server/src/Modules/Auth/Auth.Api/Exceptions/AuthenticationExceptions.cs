namespace Auth.Api.Exceptions;

public class AuthenticatedUserNotFoundException : Exception
{
    public AuthenticatedUserNotFoundException(Guid userId)
        : base($"User's credentials are signed and the signature is valid, but user with ID '{userId}' was not found in the database.")
    {
    }
}