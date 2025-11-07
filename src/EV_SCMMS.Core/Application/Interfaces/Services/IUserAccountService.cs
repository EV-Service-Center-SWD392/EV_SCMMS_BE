using EV_SCMMS.Core.Application.DTOs.User;
using EV_SCMMS.Core.Application.Interfaces;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Service interface for user account management
/// </summary>
public interface IUserAccountService
{
    Task<IServiceResult<UserAccountDto>> CreateAsync(CreateUserAccountDto dto);
    Task<IServiceResult<UserAccountDto>> UpdateAsync(Guid id, UpdateUserAccountDto dto);
    Task<IServiceResult<UserAccountDto>> UpdateRoleAsync(Guid id, Guid roleId);
    Task<IServiceResult<bool>> DeleteAsync(Guid id);
    Task<IServiceResult<UserAccountDto>> GetByIdAsync(Guid id);
    Task<IServiceResult<List<UserAccountDto>>> GetAllAsync();
    Task<IServiceResult<List<UserAccountDto>>> GetByRoleAsync(string role);
    Task<IServiceResult<List<TechnicianWithCertificatesDto>>> GetAllTechniciansAsync();
}