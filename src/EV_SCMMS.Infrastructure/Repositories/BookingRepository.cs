
using EV_SCMMS.Application.Interfaces;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Persistence;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(BookingThaoNtt booking) // <-- Tên đúng
    {
        await _context.BookingThaoNtts.AddAsync(booking); // <-- Chỗ này cũng phải đúng tên DbSet trong DbContext
        await _context.SaveChangesAsync();
    }
}