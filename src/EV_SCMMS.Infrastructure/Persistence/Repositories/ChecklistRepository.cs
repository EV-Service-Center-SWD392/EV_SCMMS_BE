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

        var existing = await _context.Checklistresponsethaontts
            .Where(r => r.Intakeid == intakeId && itemIds.Contains(r.Itemid))
            .ToListAsync(ct);

        var now = DateTime.UtcNow;

        foreach (var payload in responseList)
        {
            var entity = existing.FirstOrDefault(r => r.Itemid == payload.ItemId);
            if (entity != null)
            {
                entity.Valuetext = payload.Value;
                entity.Comment = payload.Note;
                entity.Photourl = payload.PhotoUrl;
                entity.Isactive = true;
                entity.Updatedat = now;
            }
            else
            {
                _context.Checklistresponsethaontts.Add(new Checklistresponsethaontt
                {
                    Responseid = Guid.NewGuid(),
                    Intakeid = intakeId,
                    Itemid = payload.ItemId,
                    Valuetext = payload.Value,
                    Comment = payload.Note,
                    Photourl = payload.PhotoUrl,
                    Severity = null,
                    Status = "RECORDED",
                    Isactive = true,
                    Createdat = now,
                    Updatedat = now
                });
            }
        }
    }
}
