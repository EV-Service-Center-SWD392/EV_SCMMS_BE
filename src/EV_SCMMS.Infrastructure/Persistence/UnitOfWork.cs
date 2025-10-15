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
    private IUserRepository? _userRepository;

    private IMaintenanceHistoryDungVmRepository? _maintenanceHistoryDungVmRepository;
    private IMaintenanceTaskDungVmRepository? _maintenanceTaskDungVmRepository;
    private IVehicleConditionDungVmRepository? _vehicleConditionDungVmRepository;
    private IOrderServiceThaoNttRepository? _orderServiceThaoNttRepository;
    private IOrderThaoNttRepository? _orderThaoNttRepository;


    public IUserRepository UserRepository => throw new NotImplementedException();

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    // public IUserRepository UserRepository
    // {
    //     get
    //     {
    //         _userRepository ??= new UserRepository(_context);
    //         return _userRepository;
    //     }
    // }


    public IMaintenanceHistoryDungVmRepository MaintenanceHistoryDungVmRepository
    {
        get
        {
            _maintenanceHistoryDungVmRepository ??= new MaintenanceHistoryDungVmRepository(_context);
            return _maintenanceHistoryDungVmRepository;
        }
    }
    public IMaintenanceTaskDungVmRepository MaintenanceTaskDungVmRepository
    {
        get
        {
            _maintenanceTaskDungVmRepository ??= new MaintenanceTaskDungVmRepository(_context);
            return _maintenanceTaskDungVmRepository;
        }
    }
    public IVehicleConditionDungVmRepository VehicleConditionDungVmRepository
    {
        get
        {
            _vehicleConditionDungVmRepository ??= new VehicleConditionDungVmRepository(_context);
            return _vehicleConditionDungVmRepository;
        }
    }   
    public IOrderServiceThaoNttRepository OrderServiceThaoNttRepository 
    {
        get
        {
            _orderServiceThaoNttRepository ??= new OrderServiceThaoNttRepository(_context);
            return _orderServiceThaoNttRepository;
        }
    }
    public IOrderThaoNttRepository orderThaoNttRepository
    {
        get
        {
            _orderThaoNttRepository ??= new OrderThaoNttRepository(_context);
            return _orderThaoNttRepository;
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
