using Auth.Domain.Entities;
using Auth.Domain.Tests.Builders;

namespace Auth.Domain.Tests.Entities;

public class SessionTests
{
    [Fact]
    public void Create_Should_Create_Session_When_All_Inputs_Are_Valid()
    {
        var ipAddress = "192.168.100.1";
        
        var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/58.0.3029.110 Safari/537.3";

        var userValid = new UserBuilder().Build();
        
        var before = DateTime.UtcNow;

        var result = Session.Create(userValid, ipAddress, userAgent);

        var after = DateTime.UtcNow;

        Assert.True(result.IsSuccess);

        var session = result.Value;

        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.Equal(userValid.Id, session.UserId);
        Assert.Equal(userValid, session.User);

        Assert.True(session.CreatedAt >= before);
        Assert.True(session.CreatedAt <= after);

        Assert.True(session.ExpiresAt >= before.AddDays(7));
        Assert.True(session.ExpiresAt <= after.AddDays(7));

        Assert.Null(session.RevokedAt);
    }
}