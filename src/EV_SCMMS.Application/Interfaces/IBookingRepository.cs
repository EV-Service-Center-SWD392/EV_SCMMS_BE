namespace EV_SCMMS.Application.Interfaces; // <-- PHẢI CÓ DÒNG NÀY

using EV_SCMMS.Core.Domain.Models; // <-- Dùng địa chỉ của BookingThaoNtt

public interface IBookingRepository
{
    // Sửa Booking thành BookingThaoNtt
    Task AddAsync(BookingThaoNtt booking);
}