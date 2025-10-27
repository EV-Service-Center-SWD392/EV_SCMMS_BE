using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EV_SCMMS.Core.Application.DTOs.ChecklistItems;
using EV_SCMMS.Core.Application.DTOs.Common;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Services;

public class ChecklistItemService : IChecklistItemService
{
    private readonly IUnitOfWork _uow;

    public ChecklistItemService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<PagedResult<ChecklistItemDto>> GetAsync(string? q, string status, int page, int pageSize, string sort, string order, CancellationToken ct)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : (pageSize > 200 ? 200 : pageSize);

        var qry = _uow.ChecklistItemRepository.Query().AsNoTracking();

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

        qry = (sort?.ToLower()) switch
        {
            "createdat" => (order.Equals("asc", StringComparison.OrdinalIgnoreCase) ? qry.OrderBy(x => x.Createdat) : qry.OrderByDescending(x => x.Createdat)),
            "updatedat" => (order.Equals("asc", StringComparison.OrdinalIgnoreCase) ? qry.OrderBy(x => x.Updatedat) : qry.OrderByDescending(x => x.Updatedat)),
            "code" => (order.Equals("asc", StringComparison.OrdinalIgnoreCase) ? qry.OrderBy(x => x.Code) : qry.OrderByDescending(x => x.Code)),
            "name" => (order.Equals("asc", StringComparison.OrdinalIgnoreCase) ? qry.OrderBy(x => x.Name) : qry.OrderByDescending(x => x.Name)),
            _ => qry.OrderByDescending(x => x.Createdat)
        };

        var total = await qry.CountAsync(ct);
        var skip = (page - 1) * pageSize;
        var items = await qry.Skip(skip).Take(pageSize).ToListAsync(ct);

        return new PagedResult<ChecklistItemDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ChecklistItemDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var e = await _uow.ChecklistItemRepository.FindByIdAsync(id, ct);
        return e == null ? null : MapToDto(e);
    }

    public async Task<ChecklistItemDto> CreateAsync(CreateChecklistItemDto dto, Guid actorId, CancellationToken ct)
    {
        ValidateType(dto.Type);
        if (!string.IsNullOrWhiteSpace(dto.Code))
        {
            var exists = await _uow.ChecklistItemRepository.CodeExistsAsync(dto.Code!, null, ct);
            if (exists) throw new ArgumentException("Code already exists");
        }

        var now = DateTime.UtcNow;
        var entity = new Checklistitemthaontt
        {
            Itemid = Guid.NewGuid(),
            Code = string.IsNullOrWhiteSpace(dto.Code) ? null : dto.Code!.Trim(),
            Name = dto.Name.Trim(),
            Type = dto.Type,
            Unit = string.IsNullOrWhiteSpace(dto.Unit) ? null : dto.Unit!.Trim(),
            Status = "ACTIVE",
            Isactive = true,
            Createdat = now,
            Updatedat = now
        };

        await _uow.ChecklistItemRepository.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task<ChecklistItemDto> UpdateAsync(Guid id, UpdateChecklistItemDto dto, Guid actorId, CancellationToken ct)
    {
        var entity = await _uow.ChecklistItemRepository.FindByIdAsync(id, ct);
        if (entity == null) throw new KeyNotFoundException("Item not found");

        if (dto.Type.HasValue) ValidateType(dto.Type.Value);

        if (dto.Code != null)
        {
            var code = dto.Code.Trim();
            if (code.Length == 0)
            {
                entity.Code = null;
            }
            else
            {
                var exists = await _uow.ChecklistItemRepository.CodeExistsAsync(code, id, ct);
                if (exists) throw new ArgumentException("Code already exists");
                entity.Code = code;
            }
        }

        if (dto.Name != null) entity.Name = dto.Name.Trim();
        if (dto.Type.HasValue) entity.Type = dto.Type.Value;
        if (dto.Unit != null) entity.Unit = string.IsNullOrWhiteSpace(dto.Unit) ? null : dto.Unit.Trim();

        if (dto.IsActive.HasValue)
        {
            entity.Isactive = dto.IsActive.Value;
            entity.Status = dto.IsActive.Value ? "ACTIVE" : "INACTIVE";
        }

        if (dto.Status != null)
        {
            var st = dto.Status.ToUpperInvariant();
            if (st != "ACTIVE" && st != "INACTIVE") throw new ArgumentException("Invalid status");
            entity.Status = st;
            entity.Isactive = st == "ACTIVE";
        }

        entity.Updatedat = DateTime.UtcNow;
        await _uow.ChecklistItemRepository.UpdateAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task<bool> SoftDeleteAsync(Guid id, Guid actorId, CancellationToken ct)
    {
        var entity = await _uow.ChecklistItemRepository.FindByIdAsync(id, ct);
        if (entity == null) return false;
        entity.Isactive = false;
        entity.Status = "INACTIVE";
        entity.Updatedat = DateTime.UtcNow;
        await _uow.ChecklistItemRepository.UpdateAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<ChecklistItemDto> SetActiveAsync(Guid id, bool isActive, Guid actorId, CancellationToken ct)
    {
        var entity = await _uow.ChecklistItemRepository.FindByIdAsync(id, ct);
        if (entity == null) throw new KeyNotFoundException("Item not found");
        entity.Isactive = isActive;
        entity.Status = isActive ? "ACTIVE" : "INACTIVE";
        entity.Updatedat = DateTime.UtcNow;
        await _uow.ChecklistItemRepository.UpdateAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    private static void ValidateType(short type)
    {
        if (type < 1 || type > 3) throw new ArgumentException("Type must be 1, 2, or 3");
    }

    private static ChecklistItemDto MapToDto(Checklistitemthaontt e)
    {
        return new ChecklistItemDto
        {
            Id = e.Itemid,
            Code = e.Code,
            Name = e.Name,
            Type = (short)(e.Type ?? 0),
            Unit = e.Unit,
            Status = e.Status ?? (e.Isactive == false ? "INACTIVE" : "ACTIVE"),
            IsActive = e.Isactive ?? false,
            CreatedAt = e.Createdat,
            UpdatedAt = e.Updatedat
        };
    }
}

