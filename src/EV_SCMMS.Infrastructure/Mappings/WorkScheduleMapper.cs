using EV_SCMMS.Core.Application.DTOs.WorkSchedule;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Extension methods for WorkScheduleTuantm mapping
/// </summary>
public static class WorkScheduleMapper
{
    public static WorkScheduleDto ToDto(this WorkScheduleTuantm entity)
    {
        if (entity == null) return null;

        return new WorkScheduleDto
        {
            Id = entity.Id,
            TechnicianId = entity.TechnicianId,
            WorkDate = entity.WorkDate,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            SlotCapacity = entity.SlotCapacity,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static List<WorkScheduleDto> ToDto(this IEnumerable<WorkScheduleTuantm> entities)
    {
        if (entities == null) return new List<WorkScheduleDto>();
        return entities.Select(e => e.ToDto()).Where(x => x != null).ToList();
    }

    public static WorkScheduleTuantm ToEntity(this CreateWorkScheduleDto dto)
    {
        if (dto == null) return null;

        return new WorkScheduleTuantm
        {
            Id = Guid.NewGuid(),
            TechnicianId = dto.TechnicianId,
            WorkDate = dto.WorkDate,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            SlotCapacity = dto.SlotCapacity,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateFromDto(this WorkScheduleTuantm entity, UpdateWorkScheduleDto dto)
    {
        if (entity == null || dto == null) return;

        entity.TechnicianId = dto.TechnicianId;
        entity.WorkDate = dto.WorkDate;
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.SlotCapacity = dto.SlotCapacity;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}

