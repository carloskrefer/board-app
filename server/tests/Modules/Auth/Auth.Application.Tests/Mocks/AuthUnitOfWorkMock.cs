using Auth.Application.Interfaces;
using TestsCommon.Mocks.Helpers;

namespace Auth.Application.Tests.Mocks;

public class AuthUnitOfWorkMock : IAuthUnitOfWork
{
    public CallTracker Tracker { get; } = new();

    public Func<IEnumerable<object>, CancellationToken, Task<Result<int>>> CommitAsyncHandler { get; set; } = 
        (_, _) => Task.FromResult(Result.Success(1));
    public Task<Result<int>> CommitAsync(IEnumerable<object> aggregateRootsTouched, CancellationToken ct) {
        Tracker.Record([ct]);
        return CommitAsyncHandler(aggregateRootsTouched, ct);
    }
}