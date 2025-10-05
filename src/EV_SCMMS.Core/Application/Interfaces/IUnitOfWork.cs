namespace EV_SCMMS.Core.Application.Interfaces;

/// <summary>
/// Unit of Work interface for transaction management
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Save all changes made in this context to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin a database transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
