using EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;
using EV_SCMMS.Core.Application.Interfaces;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Service interface for managing user work schedule assignments
/// </summary>
public interface IUserWorkScheduleService
{
    Task<IServiceResult<UserWorkScheduleDto>> CreateAsync(CreateUserWorkScheduleDto dto);
    Task<IServiceResult<UserWorkScheduleDto>> UpdateAsync(Guid id, UpdateUserWorkScheduleDto dto);
    Task<IServiceResult<bool>> DeleteAsync(Guid id);
    Task<IServiceResult<UserWorkScheduleDto>> GetByIdAsync(Guid id);
    Task<IServiceResult<List<UserWorkScheduleDto>>> GetByUserIdAsync(Guid userId);
    Task<IServiceResult<List<UserWorkScheduleDto>>> GetByWorkScheduleIdAsync(Guid workScheduleId);
    Task<IServiceResult<List<UserWorkScheduleDto>>> GetByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<IServiceResult<bool>> IsUserAvailableAsync(Guid userId, Guid workScheduleId);
    Task<IServiceResult<AssignmentResultDto>> BulkAssignAsync(BulkAssignTechniciansDto dto);
    Task<IServiceResult<AssignmentResultDto>> AutoAssignAsync(AutoAssignRequestDto dto);
    Task<IServiceResult<List<UserWorkScheduleDto>>> GetConflictingAssignmentsAsync(Guid userId, DateTime startTime, DateTime endTime);
    Task<IServiceResult<TechnicianWorkloadDto>> GetTechnicianWorkloadAsync(Guid userId, DateTime date);
    Task<IServiceResult<List<TechnicianScheduleDto>>> GetAllTechniciansWithSchedulesAsync();
}