using EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Mappings;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Service implementation for user work schedule assignments
/// </summary>
public class UserWorkScheduleService : IUserWorkScheduleService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserWorkScheduleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<UserWorkScheduleDto>> CreateAsync(CreateUserWorkScheduleDto dto)
    {
        try
        {
            var entity = dto.ToEntity();
            await _unitOfWork.UserWorkScheduleRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<UserWorkScheduleDto>.Success(entity.ToDto(), "User work schedule created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<UserWorkScheduleDto>.Failure($"Error creating user work schedule: {ex.InnerException?.Message}");
        }
    }

    public async Task<IServiceResult<UserWorkScheduleDto>> UpdateAsync(Guid id, UpdateUserWorkScheduleDto dto)
    {
        try
        {
            var existing = await _unitOfWork.UserWorkScheduleRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return ServiceResult<UserWorkScheduleDto>.Failure("User work schedule not found");
            }

            existing.UpdateFromDto(dto);
            await _unitOfWork.UserWorkScheduleRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<UserWorkScheduleDto>.Success(existing.ToDto(), "User work schedule updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<UserWorkScheduleDto>.Failure($"Error updating user work schedule: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var existing = await _unitOfWork.UserWorkScheduleRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return ServiceResult<bool>.Failure("User work schedule not found");
            }

            existing.Isactive = false;
            existing.Updatedat = DateTime.UtcNow;
            await _unitOfWork.UserWorkScheduleRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "User work schedule deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error deleting user work schedule: {ex.Message}");
        }
    }

    public async Task<IServiceResult<UserWorkScheduleDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _unitOfWork.UserWorkScheduleRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<UserWorkScheduleDto>.Failure("User work schedule not found");
            }

            return ServiceResult<UserWorkScheduleDto>.Success(entity.ToDto());
        }
        catch (Exception ex)
        {
            return ServiceResult<UserWorkScheduleDto>.Failure($"Error retrieving user work schedule: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<UserWorkScheduleDto>>> GetByUserIdAsync(Guid userId)
    {
        try
        {
            var items = await _unitOfWork.UserWorkScheduleRepository.GetByUserIdAsync(userId);
            return ServiceResult<List<UserWorkScheduleDto>>.Success(items.Select(x => x.ToDto()).ToList());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserWorkScheduleDto>>.Failure($"Error retrieving user schedules: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<UserWorkScheduleDto>>> GetByWorkScheduleIdAsync(Guid workScheduleId)
    {
        try
        {
            var items = await _unitOfWork.UserWorkScheduleRepository.GetByWorkScheduleIdAsync(workScheduleId);
            return ServiceResult<List<UserWorkScheduleDto>>.Success(items.Select(x => x.ToDto()).ToList());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserWorkScheduleDto>>.Failure($"Error retrieving work schedule assignments: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<UserWorkScheduleDto>>> GetByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var items = await _unitOfWork.UserWorkScheduleRepository.GetByDateRangeAsync(userId, startDate, endDate);
            return ServiceResult<List<UserWorkScheduleDto>>.Success(items.Select(x => x.ToDto()).ToList());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserWorkScheduleDto>>.Failure($"Error retrieving schedules by date range: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> IsUserAvailableAsync(Guid userId, Guid workScheduleId)
    {
        try
        {
            var isAvailable = await _unitOfWork.UserWorkScheduleRepository.IsUserAvailableAsync(userId, workScheduleId);
            return ServiceResult<bool>.Success(isAvailable);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error checking user availability: {ex.Message}");
        }
    }

    public async Task<IServiceResult<AssignmentResultDto>> BulkAssignAsync(BulkAssignTechniciansDto dto)
    {
        try
        {
            var result = new AssignmentResultDto();
            var assignments = new List<Userworkscheduletuantm>();

            foreach (var technicianId in dto.TechnicianIds)
            {
                var isAvailable = await _unitOfWork.UserWorkScheduleRepository.IsUserAvailableAsync(technicianId, dto.WorkScheduleId);
                if (isAvailable)
                {
                    var assignment = new CreateUserWorkScheduleDto
                    {
                        UserId = technicianId,
                        WorkScheduleId = dto.WorkScheduleId
                    }.ToEntity();
                    assignments.Add(assignment);
                }
                else
                {
                    result.FailedAssignments.Add(new AssignmentErrorDto
                    {
                        TechnicianId = technicianId,
                        ErrorMessage = "Technician not available",
                        ErrorCode = "CONFLICT"
                    });
                }
            }

            if (assignments.Any())
            {
                await _unitOfWork.UserWorkScheduleRepository.BulkAssignAsync(assignments);
                await _unitOfWork.SaveChangesAsync();
                result.SuccessfulAssignments = assignments.Select(x => x.ToDto()).ToList();
            }

            result.TotalProcessed = dto.TechnicianIds.Count;
            result.SuccessCount = result.SuccessfulAssignments.Count;
            result.FailureCount = result.FailedAssignments.Count;

            return ServiceResult<AssignmentResultDto>.Success(result, "Bulk assignment completed");
        }
        catch (Exception ex)
        {
            return ServiceResult<AssignmentResultDto>.Failure($"Error in bulk assignment: {ex.Message}");
        }
    }

    public async Task<IServiceResult<AssignmentResultDto>> AutoAssignAsync(AutoAssignRequestDto dto)
    {
        try
        {
            var workSchedule = await _unitOfWork.WorkScheduleRepository.GetByIdAsync(dto.WorkScheduleId);
            if (workSchedule == null)
            {
                return ServiceResult<AssignmentResultDto>.Failure("Work schedule not found");
            }

            var availableSchedules = await _unitOfWork.WorkScheduleRepository
                .GetSchedulesWithAvailableCapacityAsync(workSchedule.Starttime, workSchedule.Endtime, workSchedule.Centerid);

            var availableTechnicians = availableSchedules
                .SelectMany(s => s.Userworkscheduletuantms)
                .Where(u => u.Isactive == true)
                .Select(u => u.Userid)
                .Distinct()
                .Take(dto.RequiredTechnicianCount)
                .ToList();

            var bulkDto = new BulkAssignTechniciansDto
            {
                WorkScheduleId = dto.WorkScheduleId,
                TechnicianIds = availableTechnicians
            };

            return await BulkAssignAsync(bulkDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<AssignmentResultDto>.Failure($"Error in auto assignment: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<UserWorkScheduleDto>>> GetConflictingAssignmentsAsync(Guid userId, DateTime startTime, DateTime endTime)
    {
        try
        {
            var items = await _unitOfWork.UserWorkScheduleRepository.GetConflictingAssignmentsAsync(userId, startTime, endTime);
            return ServiceResult<List<UserWorkScheduleDto>>.Success(items.Select(x => x.ToDto()).ToList());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserWorkScheduleDto>>.Failure($"Error retrieving conflicts: {ex.Message}");
        }
    }

    public async Task<IServiceResult<TechnicianWorkloadDto>> GetTechnicianWorkloadAsync(Guid userId, DateTime date)
    {
        try
        {
            var assignments = await _unitOfWork.UserWorkScheduleRepository.GetTechnicianWorkloadAsync(userId, date);
            var totalHours = assignments.Sum(a => (a.Workschedule.Endtime - a.Workschedule.Starttime).TotalHours);
            
            var workload = new TechnicianWorkloadDto
            {
                TechnicianId = userId,
                AssignedSchedulesCount = assignments.Count,
                TotalWorkHours = totalHours,
                Date = date,
                IsOverloaded = totalHours > 8 // Assuming 8 hours is max per day
            };

            return ServiceResult<TechnicianWorkloadDto>.Success(workload);
        }
        catch (Exception ex)
        {
            return ServiceResult<TechnicianWorkloadDto>.Failure($"Error retrieving workload: {ex.Message}");
        }
    }
}