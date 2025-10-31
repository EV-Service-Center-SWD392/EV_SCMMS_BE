using EV_SCMMS.Core.Application.DTOs.WorkSchedule;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Extension methods for Workscheduletuantm mapping
/// </summary>
public static class WorkScheduleMapper
{
    public static WorkScheduleDto ToDto(this Workscheduletuantm entity)
    {
        if (entity == null) return null;

        return new WorkScheduleDto
        {
            WorkScheduleId = entity.Workscheduleid,
            CenterId = entity.Centerid,
            Starttime = entity.Starttime,
            Endtime = entity.Endtime,
            Status = entity.Status,
            IsActive = entity.Isactive ?? false,
            CreatedAt = entity.Createdat,
            UpdatedAt = entity.Updatedat
        };
    }

    public static List<WorkScheduleDto> ToDto(this IEnumerable<Workscheduletuantm> entities)
    {
        if (entities == null) return new List<WorkScheduleDto>();
        return entities.Select(e => e.ToDto()).Where(x => x != null).ToList();
    }

    public static Workscheduletuantm ToEntity(this CreateWorkScheduleDto dto)
    {
        if (dto == null) return null;

        return new Workscheduletuantm
        {
            Centerid = dto.CenterId,
            Starttime = dto.Starttime,
            Endtime = dto.Endtime,
        };
    }

    public static void UpdateFromDto(this Workscheduletuantm entity, UpdateWorkScheduleDto dto)
    {
        if (entity == null || dto == null) return;

        entity.Centerid = dto.CenterId;
        entity.Starttime = dto.Starttime;
        entity.Endtime = dto.Endtime;
        entity.Status = dto.Status;
        entity.Isactive = dto.IsActive;
        entity.Updatedat = DateTime.UtcNow;
    }

    public static Workscheduletuantm ToEntity(this WorkScheduleDto dto)
    {
        if (dto == null) return null;

        return new Workscheduletuantm
        {
            Centerid = dto.CenterId,
            Starttime = dto.Starttime,
            Endtime = dto.Endtime
        };
    }
}

