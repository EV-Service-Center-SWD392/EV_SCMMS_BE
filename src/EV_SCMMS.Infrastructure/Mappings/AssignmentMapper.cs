using EV_SCMMS.Core.Application.DTOs.Assignment;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Mapping extensions for Assignmentthaontt
/// </summary>
public static class AssignmentMapper
{
    public static AssignmentDto ToDto(this Assignmentthaontt entity)
    {
        if (entity == null) return null;

        // Attempt to get CenterId from navigation if present
        var centerId = entity.Booking?.Slot?.Centerid ?? Guid.Empty;

        return new AssignmentDto
        {
            Id = entity.Assignmentid,
            TechnicianId = entity.Technicianid,
            CenterId = centerId,
            PlannedStartUtc = entity.Plannedstartutc ?? default,
            PlannedEndUtc = entity.Plannedendutc ?? default,
            Status = entity.Status ?? string.Empty
        };
    }

    public static List<AssignmentDto> ToDto(this IEnumerable<Assignmentthaontt> entities)
    {
        if (entities == null) return new List<AssignmentDto>();
        return entities.Select(e => e.ToDto()).Where(x => x != null).ToList();
    }

    public static Assignmentthaontt ToEntity(this CreateAssignmentDto dto)
    {
        if (dto == null) return null;

        return new Assignmentthaontt
        {
            Assignmentid = Guid.NewGuid(),
            Bookingid = dto.BookingId,
            Technicianid = dto.TechnicianId,
            Plannedstartutc = dto.PlannedStartUtc,
            Plannedendutc = dto.PlannedEndUtc,
            Queueno = null,
            Status = "PENDING",
            Isactive = true,
            Createdat = DateTime.UtcNow,
            Updatedat = DateTime.UtcNow
        };
    }

    public static void UpdateTimes(this Assignmentthaontt entity, DateTime startUtc, DateTime endUtc)
    {
        if (entity == null) return;
        entity.Plannedstartutc = startUtc;
        entity.Plannedendutc = endUtc;
        entity.Updatedat = DateTime.UtcNow;
    }

    public static void UpdateTechnician(this Assignmentthaontt entity, Guid technicianId)
    {
        if (entity == null) return;
        entity.Technicianid = technicianId;
        entity.Updatedat = DateTime.UtcNow;
    }
}
