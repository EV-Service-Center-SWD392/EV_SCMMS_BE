namespace EV_SCMMS.Application.Bookings.Commands.CreateBooking; // <-- Đảm bảo dòng này đúng

using MediatR;

public record CreateBookingCommand(
    Guid VehicleId,
    string ServiceType,
    DateTime RequestedDate
) : IRequest<Guid>;