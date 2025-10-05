using EV_SCMMS.Core.Application.Interfaces.Repositories;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// User repository implementation
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    // Implement user-specific repository methods here
    // Example:
    // public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    // {
    //     return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    // }
}
