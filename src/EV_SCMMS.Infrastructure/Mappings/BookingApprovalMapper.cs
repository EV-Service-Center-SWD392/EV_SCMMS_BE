using System;
using EV_SCMMS.Core.Application.DTOs.BookingApproval;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Mapping helpers for booking approval workflow.
/// </summary>
public static class BookingApprovalMapper
{
    public static BookingApprovalDto ToBookingApprovalDto(this Bookinghuykt entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        return new BookingApprovalDto
        {
            Id = entity.Bookingid,
            CenterId = entity.Slot?.Centerid,
            CustomerId = entity.Customerid,
            VehicleId = entity.Vehicleid,
            ServiceTypeId = null,
            PreferredStart = entity.Slot?.Startutc,
            PreferredEnd = entity.Slot?.Endutc,
            BookingDate = entity.BookingDate,
            Status = entity.Status ?? string.Empty,
            ApprovedBy = null,
            ApprovedAt = null,
            RejectedBy = null,
            RejectedAt = null,
            RejectReason = entity.Status?.Equals("REJECTED", StringComparison.OrdinalIgnoreCase) == true ? entity.Notes : null,
            CreatedAt = entity.Createdat
        };
    }

    public static IEnumerable<BookingApprovalDto> ToBookingApprovalDto(this IEnumerable<Bookinghuykt> entities)
    {
        if (entities == null) yield break;

        foreach (var entity in entities)
        {
            yield return entity.ToBookingApprovalDto();
        }
    }
}
