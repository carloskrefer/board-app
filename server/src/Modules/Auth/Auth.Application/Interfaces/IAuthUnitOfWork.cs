namespace Auth.Application.Interfaces;

/// <summary>
/// Interface apenas para fins de injeção de dependência: há vários <see cref="DbContext"/> no projeto que implementam 
/// o <see cref="IOptimisticUnitOfWork"/>, um para cada módulo. Por causa disso, foi necessário uma interface 
/// <see cref="IOptimisticUnitOfWork"/> específica para o módulo atual, pra utilizar o <see cref="DbContext"/> dele.
/// </summary>
public interface IAuthUnitOfWork : IOptimisticUnitOfWork {}