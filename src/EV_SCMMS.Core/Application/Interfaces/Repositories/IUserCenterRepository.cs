using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for user-center membership operations
/// </summary>
public interface IUserCenterRepository : IGenericRepository<Usercentertuantm>
{
    Task<bool> IsUserInCenterAsync(Guid userId, Guid centerId, CancellationToken ct = default);
}

