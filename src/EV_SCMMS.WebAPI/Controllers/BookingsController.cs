
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Application.Bookings.Commands.CreateBooking;

namespace EV_SCMMS.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")] // URL sẽ là /api/bookings
public class BookingsController : ControllerBase
{
    private readonly ISender _mediator;

    // Constructor để inject MediatR vào
    public BookingsController(ISender mediator)
    {
        _mediator = mediator;
    }

    // Đây chính là API endpoint của ông
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
    {
        // Controller không làm gì cả, chỉ ném command cho MediatR
        var bookingId = await _mediator.Send(command);

        // Trả về 200 OK cùng với ID của booking vừa tạo
        return Ok(new { bookingId = bookingId });
    }
}