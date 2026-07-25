/// <summary>
/// Representa uma unidade de trabalho (Unit of Work).
/// </summary>
/// <remarks>
/// <para>
/// Unidade de trabalho é um padrão de design que mantém um registro de todas as operações realizadas durante uma 
/// transação. O objetivo é garantir que todas as operações sejam concluídas com sucesso ou, em caso de falha, que 
/// nenhuma mudança seja persistida.
/// </para>
/// <para>
/// Caso utilize uma única instância do DbContext (Entity Framework) por request, saiba que ele já implementa o padrão 
/// de unidade de trabalho. Esta interface, neste caso, só serviria para tornar o código mais legível.
/// </para>
/// <para>
/// Para saber mais sobre o padrão Unit of Work, consultar documento
/// <see href="https://docs.google.com/document/d/1dfOAf1fsHn9sSngZ2EsRp8kODwlFWhUqg6DQp7bY7Vc/edit?tab=t.0">
/// Unit of Work</see>.
/// </para>
/// <para>
/// O campo <see cref="aggregateRootsTouched"/> é necessário para que o método <see cref="CommitAsync"/> consiga 
/// atualizar a versão de linha (row version) de cada aggregate root modificado. Isso é necessário para implementar o 
/// padrão de controle de concorrência otimista.
/// </para>
/// </remarks>
public interface IOptimisticUnitOfWork
{
    public Task<Result<int>> CommitAsync(IEnumerable<object> aggregateRootsTouched, CancellationToken ct);
}

public static class UnitOfWorkErrors
{
    public static readonly string Id = "UNIT_OF_WORK";
    public static readonly Error OptimisticConcurrency = 
        new(
            $"{Id}.OPTIMISTIC_CONCURRENCY", 
            "Optimistic concurrency error occurred. The data has been modified by another request. " +
            "You can retry the operation.");

    public static readonly Error DatabaseError = 
        new(
            $"{Id}.DATABASE_ERROR", 
            "An error is occurred while commiting to the database. " +
            "The specific error should be found by the exception information in the logs. " +
            "The exact error is not specified likely to avoid coupling the application to the error messages of a " +
            "specific DBMS.");
}