using System;
using System.Collections.Generic;
using System.Linq;
using EV_SCMMS.Core.Application.DTOs.ServiceIntake;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Mapping helpers for Service Intake aggregate
/// </summary>
public static class ServiceIntakeMapper
{
    public static ServiceIntakeDto ToDto(this Serviceintakethaontt entity)
    {
        if (entity == null) return null!;

        var booking = entity.Booking;
        var centerId = booking?.Slot?.Centerid ?? Guid.Empty;
        var vehicleId = booking?.Vehicleid ?? Guid.Empty;
        var assignmentId = ResolveAssignmentId(entity);
        var technicianId = ResolveTechnicianId(entity);

        var createdAt = EnsureUtc(entity.Createdat);
        var updatedAt = EnsureNullableUtc(entity.Updatedat);

        return new ServiceIntakeDto
        {
            Id = entity.Intakeid,
            CenterId = centerId,
            VehicleId = vehicleId,
            TechnicianId = technicianId,
            AssignmentId = assignmentId,
            BookingId = entity.Bookingid,
            Odometer = entity.Odometerkm,
            BatteryPercent = entity.Batterysoc,
            Notes = entity.Notes,
            Status = entity.Status ?? string.Empty,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public static List<ServiceIntakeDto> ToDto(this IEnumerable<Serviceintakethaontt> entities)
    {
        if (entities == null) return new List<ServiceIntakeDto>();
        return entities
            .Select(e => e.ToDto())
            .Where(dto => dto != null)
            .ToList()!;
    }

    public static Serviceintakethaontt ToEntity(this CreateServiceIntakeDto dto)
    {
        if (dto == null) return null!;

        var now = DateTime.UtcNow;

        return new Serviceintakethaontt
        {
            Intakeid = Guid.NewGuid(),
            Bookingid = dto.BookingId,
            Checkintimeutc = now,
            Odometerkm = dto.Odometer,
            Batterysoc = dto.BatteryPercent,
            Notes = dto.Notes,
            Status = "CHECKED_IN",
            Isactive = true,
            Createdat = now,
            Updatedat = now
        };
    }

    public static void UpdateFromDto(this Serviceintakethaontt entity, UpdateServiceIntakeDto dto)
    {
        if (entity == null || dto == null) return;

        entity.Odometerkm = dto.Odometer;
        entity.Batterysoc = dto.BatteryPercent;
        entity.Notes = dto.Notes;
        entity.Updatedat = DateTime.UtcNow;
    }

    private static Guid? ResolveAssignmentId(Serviceintakethaontt entity)
    {
        var bookingAssignments = entity.Booking?.Assignmentthaontts;
        if (bookingAssignments == null || bookingAssignments.Count == 0)
        {
            return null;
        }

        // Choose first active assignment as the linked assignment for the booking
        var first = bookingAssignments.FirstOrDefault(a => a.Isactive != false);
        return first?.Assignmentid;
    }

    private static Guid ResolveTechnicianId(Serviceintakethaontt entity)
    {
        var bookingAssignments = entity.Booking?.Assignmentthaontts;
        if (bookingAssignments == null || bookingAssignments.Count == 0)
        {
            return Guid.Empty;
        }

        var active = bookingAssignments.FirstOrDefault(a => a.Isactive != false);
        return active?.Technicianid ?? Guid.Empty;
    }

    private static DateTime EnsureUtc(DateTime? value)
    {
        var resolved = value ?? DateTime.UtcNow;
        return resolved.Kind == DateTimeKind.Utc
            ? resolved
            : DateTime.SpecifyKind(resolved, DateTimeKind.Utc);
    }

    private static DateTime? EnsureNullableUtc(DateTime? value)
    {
        if (!value.HasValue) return null;
        var resolved = value.Value;
        return resolved.Kind == DateTimeKind.Utc
            ? resolved
            : DateTime.SpecifyKind(resolved, DateTimeKind.Utc);
    }
}
