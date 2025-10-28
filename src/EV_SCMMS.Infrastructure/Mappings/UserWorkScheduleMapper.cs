using EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Extension methods for UserWorkSchedule entity mapping
/// </summary>
public static class UserWorkScheduleMapperExtensions
{
    /// <summary>
    /// Map Userworkscheduletuantm entity to UserWorkScheduleDto
    /// </summary>
    public static UserWorkScheduleDto ToDto(this Userworkscheduletuantm entity)
    {
        if (entity == null) return null;

        return new UserWorkScheduleDto
        {
            UserWorkScheduleId = entity.Userworkscheduleid,
            UserId = entity.Userid,
            WorkScheduleId = entity.Workscheduleid,
            Status = entity.Status,
            IsActive = entity.Isactive ?? false,
            CreatedAt = entity.Createdat,
            UpdatedAt = entity.Updatedat,
            UserName = entity.User?.Firstname + " " + entity.User?.Lastname,
            WorkScheduleStartTime = entity.Workschedule?.Starttime,
            WorkScheduleEndTime = entity.Workschedule?.Endtime
        };
    }

    /// <summary>
    /// Map CreateUserWorkScheduleDto to Userworkscheduletuantm entity
    /// </summary>
    public static Userworkscheduletuantm ToEntity(this CreateUserWorkScheduleDto createDto)
    {
        if (createDto == null) return null;

        return new Userworkscheduletuantm
        {
            Userworkscheduleid = Guid.NewGuid(),
            Userid = createDto.UserId,
            Workscheduleid = createDto.WorkScheduleId,
            Status = createDto.Status,
            Isactive = true,
            Createdat = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Update existing entity with UpdateUserWorkScheduleDto data
    /// </summary>
    public static void UpdateFromDto(this Userworkscheduletuantm entity, UpdateUserWorkScheduleDto updateDto)
    {
        if (entity == null || updateDto == null) return;

        entity.Status = updateDto.Status;
        entity.Isactive = updateDto.IsActive;
        entity.Updatedat = DateTime.UtcNow;
    }
}