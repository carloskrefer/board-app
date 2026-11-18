/// <summary>
/// Represents a unit of work (Unit of Work).
/// </summary>
/// <remarks>
/// <para>
/// Unit of Work is a design pattern that keeps a record of all operations performed during a
/// transaction. The goal is to ensure that all operations are completed successfully or, in case of failure, that
/// no changes are persisted.
/// </para>
/// <para>
/// If you use a single instance of DbContext (Entity Framework) per request, be aware that it already implements the
/// Unit of Work pattern. In this case, this interface would only serve to make the code more readable.
/// </para>
/// <para>
/// To learn more about the Unit of Work pattern, refer to the document
/// <see href="https://docs.google.com/document/d/1dfOAf1fsHn9sSngZ2EsRp8kODwlFWhUqg6DQp7bY7Vc/edit?tab=t.0">
/// Unit of Work</see>.
/// </para>
/// <para>
/// The <see cref="aggregateRootsTouched"/> field is necessary so that the <see cref="CommitAsync"/> method can
/// update the row version of each modified aggregate root. This is necessary to implement the
/// optimistic concurrency control pattern.
/// </para>
/// </remarks>
public interface IOptimisticUnitOfWork
{
    public Task<Result<int>> CommitAsync(IEnumerable<object> aggregateRootsTouched, CancellationToken ct);
}

public static class OptimisticUnitOfWorkErrors
{
    public static readonly string Id = "OPTIMISTIC_UNIT_OF_WORK";

    /// <summary>
    /// Unexpected numbers of rows changed (maybe zero).
    /// </summary>
    /// <remarks>
    /// <para>
    /// This error is used to indicate that the number of rows updated in the database does not
    /// match the expected number of rows. This can happen when using optimistic concurrency control, where
    /// multiple transactions are allowed to access the same data concurrently, but only one transaction can
    /// successfully commit its changes. If another transaction has modified the data since it was last read,
    /// the update will fail and this error will be returned.
    /// </para>
    /// <para>
    /// This error is also returned when the number of rows updated is zero, which can happen if the data has been deleted or if the update statement did not match any rows in the database.
    /// </para>
    /// </remarks>
    public static readonly Error UpdateConcurrency = 
        new(
            $"{Id}.UPDATE_CONCURRENCY", 
            "The data has been concurrently modified by another request. You can reload and retry the operation.");
}