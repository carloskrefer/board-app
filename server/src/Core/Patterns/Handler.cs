/// <summary>
/// Define o contrato padrão para execução de um caso de uso da aplicação.
/// </summary>
/// <remarks>
/// <para>
/// Todo caso de uso deve implementar diretamente
/// <see cref="IHandler{TInput, TOutput}"/>, representando uma operação
/// específica da aplicação.
/// </para>
///
/// <para>
/// Os consumidores (controllers, endpoints, handlers e similares) devem
/// depender diretamente de <see cref="IHandler{TInput, TOutput}"/>,
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
/// public class SignInCommandHandler
///     : IHandler&lt;SignInRequest, SignInResponse&gt;
/// {
///     public Task&lt;SignInResponse&gt; HandleAsync(SignInRequest input)
///     {
///         ...
///     }
/// }
/// </code>
///
/// <para>Evite criar interfaces intermediárias:</para>
/// 
/// <code>
/// public interface ISignInCommandHandler
///     : IHandler&lt;SignInRequest, SignInResponse&gt;
/// {
/// }
///
/// public class SignInCommandHandler : ISignInCommandHandler
/// {
/// }
/// </code>
///
/// <para>Prefira depender diretamente do contrato genérico:</para>
/// <code>
/// IHandler&lt;SignInRequest, SignInResponse&gt; SignInCommandHandler
/// </code>
/// </example>
/// </remarks>
/// <typeparam name="TInput">
/// Tipo da entrada recebida pelo caso de uso.
/// </typeparam>
/// <typeparam name="TOutput">
/// Tipo da saída produzida pelo caso de uso.
/// </typeparam>
public interface IHandler<TInput, TOutput> where TOutput : Result
{
    public Task<TOutput> HandleAsync(TInput input, CancellationToken ct);
}