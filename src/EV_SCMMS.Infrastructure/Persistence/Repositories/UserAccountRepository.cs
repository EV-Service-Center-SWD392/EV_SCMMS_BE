using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for UserAccount entity
/// </summary>
public class UserAccountRepository : GenericRepository<Useraccount>, IUserAccountRepository
{
    public UserAccountRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Useraccount>> GetByRoleAsync(Guid role, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Useraccounts
            .Where(x => x.Roleid == role && x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<Useraccount?> GetByUsernameAsync(string firstname, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Useraccounts
            .FirstOrDefaultAsync(x => x.Firstname == firstname, cancellationToken);
    }

    public async Task<Useraccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Useraccounts
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<bool> UpdateRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _dbSet.Useraccounts
                .FirstOrDefaultAsync(x => x.Userid == userId, cancellationToken);
            if (user == null) return false;

            user.Roleid = roleId;
            user.Updatedat = DateTime.UtcNow;
            
            await _dbSet.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    Task<List<Useraccount>> IUserAccountRepository.GetByRoleAsync(string role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<Useraccount?> IUserAccountRepository.GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}