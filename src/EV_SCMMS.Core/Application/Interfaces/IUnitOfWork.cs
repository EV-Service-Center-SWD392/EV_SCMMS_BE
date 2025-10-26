using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Application.Services;

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
    /// Role repository
    /// </summary>
    IRoleRepository RoleRepository { get; }

    /// <summary>
    /// Center repository
    /// </summary>
    ICenterRepository CenterRepository { get; }

    /// <summary>
    /// Inventory repository
    /// </summary>
    IInventoryRepository InventoryRepository { get; }

    /// <summary>
    /// Sparepart repository
    /// </summary>
    ISparepartRepository SparepartRepository { get; }

    /// <summary>
    /// Sparepart Type repository
    /// </summary>
    ISparepartTypeRepository SparepartTypeRepository { get; }

    /// <summary>
    /// Sparepart Forecast repository
    /// </summary>
    ISparepartForecastRepository SparepartForecastRepository { get; }

    /// <summary>
    /// Sparepart Replenishment Request repository
    /// </summary>
    ISparepartReplenishmentRequestRepository SparepartReplenishmentRequestRepository { get; }

    /// <summary>
    /// Sparepart Usage History repository
    /// </summary>
    ISparepartUsageHistoryRepository SparepartUsageHistoryRepository { get; }

    /// <summary>
    /// Booking repository
    /// </summary>
    IBookingRepository BookingRepository { get; }

    /// <summary>
    /// Booking Schedule repository
    /// </summary>
    IBookingScheduleRepository BookingScheduleRepository { get; }

    /// <summary>
    /// Refresh Token repository
    /// </summary>
    IRefreshTokenRepository RefreshTokenRepository { get; }

    /// <summary>
    /// WorkSchedule repository
    /// </summary>
    IWorkScheduleRepository WorkScheduleRepository { get; }

    /// <summary>
    /// Assignment repository
    /// </summary>
    IAssignmentRepository AssignmentRepository { get; }

    /// <summary>
    /// Service intake repository
    /// </summary>
    IServiceIntakeRepository ServiceIntakeRepository { get; }

    /// <summary>
    /// Checklist repository
    /// </summary>
    IChecklistRepository ChecklistRepository { get; }

    /// <summary>
    /// Work Order repository
    /// </summary>
    IWorkOrderRepository WorkOrderRepository { get; }

    /// <summary>
    /// Refresh Token service
    /// </summary>
    IRefreshTokenService RefreshTokenService { get; }

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
