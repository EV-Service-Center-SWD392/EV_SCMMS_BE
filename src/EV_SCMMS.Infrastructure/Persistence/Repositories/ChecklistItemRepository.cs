using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

public class ChecklistItemRepository : IChecklistItemRepository
{
    private readonly AppDbContext _context;

    public ChecklistItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Checklistitemthaontt> Query()
    {
        return _context.Checklistitemthaontts.AsQueryable();
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        var codeLower = code.Trim().ToLower();
        return await _context.Checklistitemthaontts
            .AsNoTracking()
            .AnyAsync(e => e.Code != null && e.Code.ToLower() == codeLower && (!excludeId.HasValue || e.Itemid != excludeId.Value), ct);
    }

    public async Task AddAsync(Checklistitemthaontt entity, CancellationToken ct = default)
    {
        await _context.Checklistitemthaontts.AddAsync(entity, ct);
    }

    public Task UpdateAsync(Checklistitemthaontt entity, CancellationToken ct = default)
    {
        _context.Checklistitemthaontts.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<Checklistitemthaontt?> FindByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Checklistitemthaontts.FirstOrDefaultAsync(e => e.Itemid == id, ct);
    }
}

