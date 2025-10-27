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

    public async Task<(List<Checklistitemthaontt> Items, int Total)> SearchAsync(
        string? q,
        string status,
        int page,
        int pageSize,
        string sort,
        string order,
        CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : (pageSize > 200 ? 200 : pageSize);

        var qry = _context.Checklistitemthaontts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLower();
            qry = qry.Where(x => (x.Code != null && x.Code.ToLower().Contains(term)) || x.Name.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(status) && !status.Equals("ALL", StringComparison.OrdinalIgnoreCase))
        {
            if (status.Equals("ACTIVE", StringComparison.OrdinalIgnoreCase))
            {
                qry = qry.Where(x => x.Isactive == true);
            }
            else if (status.Equals("INACTIVE", StringComparison.OrdinalIgnoreCase))
            {
                qry = qry.Where(x => x.Isactive == false);
            }
        }

        var sortLower = (sort ?? "createdAt").ToLower();
        var orderDesc = !string.Equals(order, "asc", StringComparison.OrdinalIgnoreCase);

        qry = sortLower switch
        {
            "createdat" => orderDesc ? qry.OrderByDescending(x => x.Createdat) : qry.OrderBy(x => x.Createdat),
            "updatedat" => orderDesc ? qry.OrderByDescending(x => x.Updatedat) : qry.OrderBy(x => x.Updatedat),
            "code" => orderDesc ? qry.OrderByDescending(x => x.Code) : qry.OrderBy(x => x.Code),
            "name" => orderDesc ? qry.OrderByDescending(x => x.Name) : qry.OrderBy(x => x.Name),
            _ => qry.OrderByDescending(x => x.Createdat)
        };

        var total = await qry.CountAsync(ct);
        var skip = (page - 1) * pageSize;
        var items = await qry.Skip(skip).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }
}

