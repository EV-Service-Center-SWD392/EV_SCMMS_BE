using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Application.Services;
using EV_SCMMS.Infrastructure.Persistence.Repositories;
using EV_SCMMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EV_SCMMS.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation for transaction management
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;
    private IDbContextTransaction? _transaction;
    
    // Repository instances
    private IUserRepository? _userRepository;
    private IRoleRepository? _roleRepository;
    private ICenterRepository? _centerRepository;
    private IInventoryRepository? _inventoryRepository;
    private ISparepartRepository? _sparepartRepository;
    private ISparepartTypeRepository? _sparepartTypeRepository;
    private ISparepartForecastRepository? _sparepartForecastRepository;
    private ISparepartReplenishmentRequestRepository? _sparepartReplenishmentRequestRepository;
    private ISparepartUsageHistoryRepository? _sparepartUsageHistoryRepository;
    private IBookingRepository? _bookingRepository;
    private IBookingScheduleRepository? _bookingScheduleRepository;
    private IRefreshTokenRepository? _refreshTokenRepository;
    private ITransactionRepository? _transactionRepository;
    private IOrderRepository? _orderRepository;
    private IReceiptRepository? _receiptRepository;
    private IReceiptItemRepository? _receiptItemRepository;
    private IWorkScheduleRepository? _workScheduleRepository;
    private IUserWorkScheduleRepository? _userWorkScheduleRepository;
    private IUserAccountRepository? _userAccountRepository;
    private IAssignmentRepository? _assignmentRepository;
    private IServiceIntakeRepository? _serviceIntakeRepository;
    private IChecklistRepository? _checklistRepository;
    private IChecklistItemRepository? _checklistItemRepository;
    private IWorkOrderRepository? _workOrderRepository;
    private IUserCenterRepository? _userCenterRepository;
    private ICertificateRepository? _certificateRepository;
    private IUserCertificateRepository? _userCertificateRepository;
    private IUserRoleRepository? _userRoleRepository;
    
    // Service instances
    private IRefreshTokenService? _refreshTokenService;

    public UnitOfWork(AppDbContext context, IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _context = context;
        _configuration = configuration;
        _loggerFactory = loggerFactory;
    }

    public IUserRepository UserRepository
    {
        get
        {
            _userRepository ??= new UserRepository(_context);
            return _userRepository;
        }
    }

    public IRoleRepository RoleRepository
    {
        get
        {
            _roleRepository ??= new RoleRepository(_context);
            return _roleRepository;
        }
    }

    public ICenterRepository CenterRepository
    {
        get
        {
            _centerRepository ??= new CenterRepository(_context);
            return _centerRepository;
        }
    }

    public IInventoryRepository InventoryRepository
    {
        get
        {
            _inventoryRepository ??= new InventoryRepository(_context);
            return _inventoryRepository;
        }
    }

    public ISparepartRepository SparepartRepository
    {
        get
        {
            _sparepartRepository ??= new SparepartRepository(_context);
            return _sparepartRepository;
        }
    }

    public ISparepartTypeRepository SparepartTypeRepository
    {
        get
        {
            _sparepartTypeRepository ??= new SparepartTypeRepository(_context);
            return _sparepartTypeRepository;
        }
    }

    public ISparepartForecastRepository SparepartForecastRepository
    {
        get
        {
            _sparepartForecastRepository ??= new SparepartForecastRepository(_context);
            return _sparepartForecastRepository;
        }
    }

    public ISparepartReplenishmentRequestRepository SparepartReplenishmentRequestRepository
    {
        get
        {
            _sparepartReplenishmentRequestRepository ??= new SparepartReplenishmentRequestRepository(_context);
            return _sparepartReplenishmentRequestRepository;
        }
    }

    public ISparepartUsageHistoryRepository SparepartUsageHistoryRepository
    {
        get
        {
            _sparepartUsageHistoryRepository ??= new SparepartUsageHistoryRepository(_context);
            return _sparepartUsageHistoryRepository;
        }
    }

    public IBookingRepository BookingRepository
    {
        get
        {
            _bookingRepository ??= new BookingRepository(_context);
            return _bookingRepository;
        }
    }

    public IBookingScheduleRepository BookingScheduleRepository
    {
        get
        {
            _bookingScheduleRepository ??= new BookingScheduleRepository(_context);
            return _bookingScheduleRepository;
        }
    }

    public IRefreshTokenRepository RefreshTokenRepository
    {
        get
        {
            _refreshTokenRepository ??= new RefreshTokenRepository(_context);
            return _refreshTokenRepository;
        }
    }

    public ITransactionRepository TransactionRepository
    {
        get
        {
            _transactionRepository ??= new TransactionRepository(_context);
            return _transactionRepository;
        }
    }

    public IOrderRepository OrderRepository
    {
        get
        {
            _orderRepository ??= new OrderRepository(_context);
            return _orderRepository;
        }
    }

    public IReceiptRepository ReceiptRepository
    {
        get
        {
            _receiptRepository ??= new ReceiptRepository(_context);
            return _receiptRepository;
        }
    }

    public IReceiptItemRepository ReceiptItemRepository
    {
        get
        {
            _receiptItemRepository ??= new ReceiptItemRepository(_context);
            return _receiptItemRepository;
        }
    }

    public IWorkScheduleRepository WorkScheduleRepository
    {
        get
        {
            _workScheduleRepository ??= new WorkScheduleRepository(_context);
            return _workScheduleRepository;
        }
    }

    public IUserWorkScheduleRepository UserWorkScheduleRepository
    {
        get
        {
            _userWorkScheduleRepository ??= new UserWorkScheduleRepository(_context);
            return _userWorkScheduleRepository;
        }
    }

    public IUserAccountRepository UserAccountRepository
    {
        get
        {
            _userAccountRepository ??= new UserAccountRepository(_context);
            return _userAccountRepository;
        }
    }

    public IAssignmentRepository AssignmentRepository
    {
        get
        {
            _assignmentRepository ??= new AssignmentRepository(_context);
            return _assignmentRepository;
        }
    }

    public IServiceIntakeRepository ServiceIntakeRepository
    {
        get
        {
            _serviceIntakeRepository ??= new ServiceIntakeRepository(_context);
            return _serviceIntakeRepository;
        }
    }

    public IChecklistRepository ChecklistRepository
    {
        get
        {
            _checklistRepository ??= new ChecklistRepository(_context);
            return _checklistRepository;
        }
    }

    public IChecklistItemRepository ChecklistItemRepository
    {
        get
        {
            _checklistItemRepository ??= new ChecklistItemRepository(_context);
            return _checklistItemRepository;
        }
    }

    public IWorkOrderRepository WorkOrderRepository
    {
        get
        {
            _workOrderRepository ??= new WorkOrderRepository(_context);
            return _workOrderRepository;
        }
    }

    public IUserCenterRepository UserCenterRepository
    {
        get
        {
            _userCenterRepository ??= new UserCenterRepository(_context);
            return _userCenterRepository;
        }
    }

    public IRefreshTokenService RefreshTokenService
    {
        get
        {
            _refreshTokenService ??= new RefreshTokenService(this, _configuration, _loggerFactory.CreateLogger<RefreshTokenService>());
            return _refreshTokenService;
        }
    }

    public ICertificateRepository CertificateRepository
    {
        get
        {
            _certificateRepository ??= new CertificateRepository(_context);
            return _certificateRepository;
        }
    }

    public IUserCertificateRepository UserCertificateRepository
    {
        get
        {
            _userCertificateRepository ??= new UserCertificateRepository(_context);
            return _userCertificateRepository;
        }
    }

    public IUserRoleRepository UserRoleRepository
    {
        get
        {
            _userRoleRepository ??= new UserRoleRepository(_context);
            return _userRoleRepository;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
