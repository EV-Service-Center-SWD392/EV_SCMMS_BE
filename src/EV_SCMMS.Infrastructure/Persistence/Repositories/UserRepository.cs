using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for User entity
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get user by email address
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User entity if found</returns>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>()
            .Where(u => u.Email.ToLower() == email.ToLower() && u.Isactive == true)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Check if email already exists
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if email exists</returns>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>()
            .AnyAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    /// <summary>
    /// Get user with role information
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User with role</returns>
    public async Task<User?> GetUserWithRoleAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>()
            .Include(u => u.Role)
            .Where(u => u.Userid == userId && u.Isactive == true)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Get user by email with role information
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User with role</returns>
    public async Task<User?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>()
            .Include(u => u.Role)
            .Where(u => u.Email.ToLower() == email.ToLower() && u.Isactive == true)
            .FirstOrDefaultAsync(cancellationToken);
    }

  public async Task<bool> PhoneExistsAsync(string phone, CancellationToken cancellationToken = default)
  {
    return await _context.Set<User>()
           .AnyAsync(u => u.Phonenumber == phone.ToLower(), cancellationToken);
  }
}