/// <summary>
/// Define o contrato padrão para execução de um caso de uso da aplicação.
/// </summary>
/// <remarks>
/// <para>
/// Todo caso de uso deve implementar diretamente
/// <see cref="IUseCase{TInput, TOutput}"/>, representando uma operação
/// específica da aplicação.
/// </para>
///
/// <para>
/// Os consumidores (controllers, endpoints, handlers e similares) devem
/// depender diretamente de <see cref="IUseCase{TInput, TOutput}"/>,
/// evitando a criação de interfaces específicas apenas para encapsular
/// este contrato.
/// </para>
///
/// <para>
/// Essa abordagem simplifica a composição da aplicação e permite a aplicação
/// de decorators genéricos para funcionalidades transversais, como logging,
/// validação, autorização, cache e métricas, sem a necessidade de criar
/// implementações específicas para cada caso de uso.
/// </para>
///
/// <para>
/// Para mais informações sobre o decorator pattern, ler documento 
/// <see href="https://docs.google.com/document/d/1kLyv7ETs559nqx4G5507EhvGGELXjgtyJsgu2gNHQCU/edit?tab=t.0">
/// Design Patterns</see>.
/// </para>
///
/// <example>
/// <para>Exemplo de uso:</para>
/// <code>
/// public class SignInService
///     : IUseCase&lt;SignInRequest, SignInResponse&gt;
/// {
///     public Task&lt;SignInResponse&gt; ExecuteAsync(SignInRequest input)
///     {
///         ...
///     }
/// }
/// </code>
///
/// <para>Evite criar interfaces intermediárias:</para>
/// 
/// <code>
/// public interface ISignInService
///     : IUseCase&lt;SignInRequest, SignInResponse&gt;
/// {
/// }
///
/// public class SignInService : ISignInService
/// {
/// }
/// </code>
///
/// <para>Prefira depender diretamente do contrato genérico:</para>
/// <code>
/// IUseCase&lt;SignInRequest, SignInResponse&gt; signInService
/// </code>
/// </example>
/// </remarks>
/// <typeparam name="TInput">
/// Tipo da entrada recebida pelo caso de uso.
/// </typeparam>
/// <typeparam name="TOutput">
/// Tipo da saída produzida pelo caso de uso.
/// </typeparam>
public interface IUseCase<TInput, TOutput> where TOutput : Result
{
    public Task<TOutput> ExecuteAsync(TInput input, CancellationToken ct);
}