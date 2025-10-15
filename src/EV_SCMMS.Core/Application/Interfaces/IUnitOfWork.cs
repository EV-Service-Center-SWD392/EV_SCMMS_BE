using EV_SCMMS.Core.Application.Interfaces.Repositories;

namespace EV_SCMMS.Core.Application.Interfaces;

/// <summary>
/// Unit of Work interface for transaction management
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// User repository
    /// </summary>
    IUserRepository UserRepository { get; }

    /// <summary>
    /// Gets the maintenance history repository.
    /// </summary>
    IMaintenanceHistoryDungVmRepository MaintenanceHistoryDungVmRepository { get; }

    /// <summary>
    /// Gets the maintenance task repository.
    /// </summary>
    IMaintenanceTaskDungVmRepository MaintenanceTaskDungVmRepository { get; }

    /// <summary>
    /// Gets the vehicle condition repository.
    /// </summary>
    IVehicleConditionDungVmRepository VehicleConditionDungVmRepository { get; }

    /// <summary>
    /// Gets the order service repository.
    /// </summary>
    IOrderServiceThaoNttRepository OrderServiceThaoNttRepository { get; }

    /// <summary>
    /// Gets the order repository.
    /// </summary>
    IOrderThaoNttRepository orderThaoNttRepository { get; }


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
