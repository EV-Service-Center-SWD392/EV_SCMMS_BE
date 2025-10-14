using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace EV_SCMMS.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation for transaction management
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;
    
    // Repository instances
    private ICenterRepository? _centerRepository;
    private IInventoryRepository? _inventoryRepository;
    private ISparepartRepository? _sparepartRepository;
    private ISparepartTypeRepository? _sparepartTypeRepository;
    private ISparepartForecastRepository? _sparepartForecastRepository;
    private ISparepartReplenishmentRequestRepository? _sparepartReplenishmentRequestRepository;
    private ISparepartUsageHistoryRepository? _sparepartUsageHistoryRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
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
