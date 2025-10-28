using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for checklist catalog and intake responses
/// </summary>
public class ChecklistRepository : IChecklistRepository
{
    private readonly AppDbContext _context;

    public ChecklistRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Checklistitemthaontt>> GetItemsAsync(CancellationToken ct = default)
    {
        return await _context.Checklistitemthaontts
            .AsNoTracking()
            .Where(item => item.Isactive != false)
            .OrderBy(item => item.Code)
            .ThenBy(item => item.Name)
            .ToListAsync(ct);
    }

    public async Task<List<Checklistresponsethaontt>> GetResponsesAsync(Guid intakeId, CancellationToken ct = default)
    {
        return await _context.Checklistresponsethaontts
            .AsNoTracking()
            .Include(r => r.Item)
            .Where(r => r.Intakeid == intakeId && r.Isactive != false)
            .ToListAsync(ct);
    }

    public async Task UpsertResponsesAsync(
        Guid intakeId,
        IEnumerable<(Guid ItemId, string? Value, string? Note, string? PhotoUrl)> responses,
        CancellationToken ct = default)
    {
        var responseList = responses.ToList();
        if (responseList.Count == 0)
        {
            return;
        }

        var itemIds = responseList.Select(r => r.ItemId).ToList();
        var items = await _context.Checklistitemthaontts
            .Where(i => itemIds.Contains(i.Itemid))
            .ToDictionaryAsync(i => i.Itemid, i => i.Type, ct);

        var existing = await _context.Checklistresponsethaontts
            .Where(r => r.Intakeid == intakeId && itemIds.Contains(r.Itemid))
            .ToListAsync(ct);

        var now = DateTime.UtcNow;

        foreach (var payload in responseList)
        {
            items.TryGetValue(payload.ItemId, out var type);

            bool? valBool = null;
            decimal? valNumber = null;
            short? severity = null;

            if (!string.IsNullOrWhiteSpace(payload.Value))
            {
                // Coerce based on item type when available; otherwise best-effort parse.
                if (type.HasValue && type.Value == 1)
                {
                    if (bool.TryParse(payload.Value, out var b)) valBool = b;
                }
                else if (type.HasValue && type.Value == 2)
                {
                    if (decimal.TryParse(payload.Value, out var d)) valNumber = d;
                }
                else if (type.HasValue && type.Value == 3)
                {
                    if (short.TryParse(payload.Value, out var s)) severity = s;
                }
                else
                {
                    // Fallback: try parse in order without enforcing type
                    if (bool.TryParse(payload.Value, out var b2)) valBool = b2;
                    else if (decimal.TryParse(payload.Value, out var d2)) valNumber = d2;
                    else if (short.TryParse(payload.Value, out var s2)) severity = s2;
                }
            }

            var entity = existing.FirstOrDefault(r => r.Itemid == payload.ItemId);
            if (entity != null)
            {
                entity.Valuetext = payload.Value;
                entity.Valuebool = valBool;
                entity.Valuenumber = valNumber;
                entity.Severity = severity;
                entity.Comment = payload.Note;
                entity.Photourl = payload.PhotoUrl;
                entity.Isactive = true;
                entity.Updatedat = now;
                entity.Status = entity.Status ?? "RECORDED";
            }
            else
            {
                _context.Checklistresponsethaontts.Add(new Checklistresponsethaontt
                {
                    Responseid = Guid.NewGuid(),
                    Intakeid = intakeId,
                    Itemid = payload.ItemId,
                    Valuetext = payload.Value,
                    Valuebool = valBool,
                    Valuenumber = valNumber,
                    Severity = severity,
                    Comment = payload.Note,
                    Photourl = payload.PhotoUrl,
                    Status = "RECORDED",
                    Isactive = true,
                    Createdat = now,
                    Updatedat = now
                });
            }
        }
    }
}
