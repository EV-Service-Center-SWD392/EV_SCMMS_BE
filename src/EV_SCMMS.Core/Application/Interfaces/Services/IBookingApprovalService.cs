using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EV_SCMMS.Core.Application.DTOs.BookingApproval;
using EV_SCMMS.Core.Application.Interfaces;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Approval workflow for booking requests.
/// </summary>
public interface IBookingApprovalService
{
    Task<IServiceResult<BookingApprovalDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IServiceResult<List<BookingApprovalDto>>> GetPendingAsync(Guid? centerId, CenterSchedulesQueryDto? dto, CancellationToken ct = default);
    Task<IServiceResult<BookingApprovalDto>> ApproveAsync(ApproveBookingDto dto, Guid staffId, CancellationToken ct = default);
    Task<IServiceResult<BookingApprovalDto>> RejectAsync(RejectBookingDto dto, Guid staffId, CancellationToken ct = default);
}
