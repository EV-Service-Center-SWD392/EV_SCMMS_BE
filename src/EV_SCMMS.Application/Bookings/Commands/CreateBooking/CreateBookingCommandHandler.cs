namespace EV_SCMMS.Application.Bookings.Commands.CreateBooking;

using MediatR;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Application.Interfaces;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IBookingRepository _bookingRepository;

    public CreateBookingCommandHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = new BookingThaoNtt
        {
            BookingThaoNttid = Guid.NewGuid(),
            VehicleId = request.VehicleId,
            Notes = $"Service Type: {request.ServiceType}",
            Status = 1, // Giả sử 1 là "Scheduled"
            CreatedAt = request.RequestedDate,
            UpdatedAt = request.RequestedDate
            // CustomerId cần được lấy từ user đang đăng nhập
        };

        // Dòng này giờ sẽ hoạt động vì interface đã được sửa đúng
        await _bookingRepository.AddAsync(booking);

        return booking.BookingThaoNttid;
    }
}