using EV_SCMMS.Core.Application.DTOs.WorkOrder;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

public static class WorkOrderMapper
{
    public static WorkOrderDto ToDto(this Workorderapprovalthaontt e)
    {
        if (e == null) return null;

        var dto = new WorkOrderDto
        {
            Id = e.WoaId,
            IntakeId = e.Order?.Booking?.Serviceintakethaontt?.Intakeid ?? Guid.Empty,
            Status = e.Status ?? string.Empty,
            Title = null, // Not persisted in current entity model
            Description = null, // Not persisted
            EstimatedAmount = null, // Not persisted
            CreatedAt = e.Createdat ?? DateTime.MinValue,
            ApprovedAt = e.Approvedat,
            StartedAt = null, // Not persisted
            CompletedAt = null, // Not persisted
            Lines = null // Not persisted
        };

        return dto;
    }

    public static List<WorkOrderDto> ToDto(this IEnumerable<Workorderapprovalthaontt> es)
    {
        if (es == null) return new List<WorkOrderDto>();
        return es.Select(e => e.ToDto()).Where(x => x != null).ToList();
    }

    public static Workorderapprovalthaontt ToEntity(this CreateWorkOrderDto dto)
    {
        if (dto == null) return null;

        var now = DateTime.UtcNow;
        return new Workorderapprovalthaontt
        {
            // Orderid will be set in service layer based on intake -> booking -> order relation
            Status = "DRAFT",
            Createdat = now,
            Updatedat = now,
            Isactive = true
        };
    }

    public static void UpdateFromDto(this Workorderapprovalthaontt e, UpdateWorkOrderDto dto)
    {
        if (e == null || dto == null) return;

        // No dedicated fields (Title/Description/EstimatedAmount/Lines) exist on entity.
        // We only update UpdatedAt to reflect modification.
        e.Updatedat = DateTime.UtcNow;
    }
}
