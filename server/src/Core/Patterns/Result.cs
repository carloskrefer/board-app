/// <summary>
/// Representa o resultado de uma operação que pode falhar por motivos
/// esperados pela aplicação.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Result"/> implementa o Result Pattern, permitindo representar
/// falhas através de valores de retorno em vez de exceções.
/// </para>
///
/// <para>
/// Esse padrão é indicado para situações esperadas, como erros de validação,
/// regras de negócio não satisfeitas ou outros cenários que fazem parte do
/// fluxo normal da aplicação. Em APIs HTTP, essas falhas normalmente são
/// convertidas em respostas da família 4xx, como <c>400 Bad Request</c>.
/// </para>
///
/// <para>
/// Exceções devem ser reservadas para falhas excepcionais, como problemas de
/// infraestrutura, indisponibilidade de serviços externos ou bugs. Esses
/// cenários normalmente representam erros da família 5xx e exigem informações
/// adicionais, como stack traces, para diagnóstico e correção.
/// </para>
///
/// <para>
/// Ao utilizar <see cref="Result"/>, os possíveis desfechos de uma operação
/// tornam-se explícitos na assinatura do método, permitindo que falhas
/// esperadas sejam tratadas de forma previsível e sem utilizar exceções como
/// mecanismo de controle de fluxo.
/// </para>
///
/// <para>
/// Para mais detalhes sobre a motivação e os benefícios do padrão, consulte
/// o documento
/// <see href="https://docs.google.com/document/d/1wKMxiOf-aYosv7sq95D_Xb9JuEdlI36VbgCeWSHTbls/edit?usp=sharing">
/// Exceptions vs Result</see>.
/// </para>
/// </remarks>
public class Result
{
    public bool IsSuccess { get; }
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new ArgumentException(
                "Success result cannot have an error different than Error.None", 
                nameof(error));

        if (!isSuccess && error == Error.None)
            throw new ArgumentException(
                "Failure result cannot have an error of Error.None", 
                nameof(error));

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsFailure => !IsSuccess;
    public static Result Success() => new (true, Error.None);
    public static Result<TValue> Success<TValue>(TValue value) => new (value, true, Error.None);
    public static Result Failure(Error error) => new (false, error);
    public static Result<TValue> Failure<TValue>(Error error) => new (default, false, error);

    public static implicit operator Result(Error error) => Failure(error);
}

/// <summary>
/// Ler <see cref="Result"/>.
/// 
/// <para>
/// Esta classe apenas a estende para adicionar um valor de retorno para o caso de sucesso.
/// </para>
/// </summary>
/// <typeparam name="TValue">Tipo do valor do retorno quando há sucesso.</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("No value present for failure.");

    public Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }

    public static implicit operator Result<TValue>(TValue value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
    
}

/// <summary>
/// Representa um erro retornado pelo <c>Result</c> quando uma operação
/// falha por um motivo não excepcional.
/// </summary>
/// <remarks>
/// <para>
/// Um <see cref="Error"/> descreve falhas esperadas da aplicação, como
/// validações inválidas, regras de negócio não satisfeitas ou outras situações
/// que normalmente resultam em respostas HTTP 400 (Bad Request).
/// </para>
///
/// <para>
/// Para falhas inesperadas, erros de infraestrutura ou bugs, utilize
/// exceções.
/// </para>
///
/// <para>
/// Cada erro é composto por um código único (<see cref="Code"/>) e uma
/// descrição opcional (<see cref="Description"/>). O código deve ser
/// estável e adequado para identificação programática, enquanto a descrição
/// destina-se à exibição ou diagnóstico.
/// </para>
///
/// <para>
/// Recomenda-se organizar os erros em classes estáticas por contexto de
/// negócio, definindo-os como constantes reutilizáveis.
/// </para>
/// 
/// <para>
/// Para representar ausência de erro utilize <see cref="None"/>.
/// </para>
/// <example>
/// Exemplo de uso:
/// <code>
/// public static class EmailErrors
/// {
///     private const string Id = "EMAIL";
///
///     public static readonly Error Format =
///         new(
///             $"{Id}.FORMAT",
///             "Invalid email address format.");
/// }
/// </code>
/// </example>
/// </remarks>
/// <param name="Code">Código único na aplicação para o erro.</param>
/// <param name="Description">Descrição do erro.</param>
public sealed record Error(string Code, string? Description = null)
{
    public static readonly Error None = new (string.Empty);
    public static readonly Error NullValue = new ("NullValue", "The value is null."); 
}

public static class ErrorExtensions
{
    public static bool HasSameCodeId(this Error error, string Id) => error.Code.StartsWith($"{Id}.");
}