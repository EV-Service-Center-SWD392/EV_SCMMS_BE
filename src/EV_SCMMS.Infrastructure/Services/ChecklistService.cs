using System;
using System.Collections.Generic;
using System.Linq;
using EV_SCMMS.Core.Application.DTOs.ServiceIntake;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Business logic for EV intake checklist catalog and responses
/// </summary>
public class ChecklistService : IChecklistService
{
    private static readonly HashSet<string> EditableStates = new(StringComparer.OrdinalIgnoreCase) { "CHECKED_IN", "INSPECTING" };
    private readonly IUnitOfWork _unitOfWork;

    public ChecklistService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<List<ChecklistItemDto>>> GetItemsAsync(CancellationToken ct = default)
    {
        try
        {
            var items = await _unitOfWork.ChecklistRepository.GetItemsAsync(ct);
            var result = items
                .Select((item, index) => new ChecklistItemDto
                {
                    Id = item.Itemid,
                    Category = ExtractCategory(item.Code),
                    Name = item.Name,
                    IsRequired = item.Type.HasValue && item.Type.Value == 1,
                    SortOrder = index
                })
                .ToList();

            return ServiceResult<List<ChecklistItemDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<ChecklistItemDto>>.Failure($"Error retrieving checklist items: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<ChecklistResponseItemDto>>> GetResponsesAsync(Guid intakeId, CancellationToken ct = default)
    {
        try
        {
            if (intakeId == Guid.Empty)
            {
                return ServiceResult<List<ChecklistResponseItemDto>>.Failure("IntakeId is required");
            }

            var responses = await _unitOfWork.ChecklistRepository.GetResponsesAsync(intakeId, ct);
            var result = responses
                .Select(r => new ChecklistResponseItemDto
                {
                    ItemId = r.Itemid,
                    Value = r.Valuetext,
                    Note = r.Comment,
                    PhotoUrl = r.Photourl
                })
                .ToList();

            return ServiceResult<List<ChecklistResponseItemDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<ChecklistResponseItemDto>>.Failure($"Error retrieving responses: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> UpsertResponsesAsync(UpsertChecklistResponsesDto dto, CancellationToken ct = default)
    {
        try
        {
            if (dto == null || dto.IntakeId == Guid.Empty)
            {
                return ServiceResult<bool>.Failure("IntakeId is required");
            }

            if (dto.Responses == null || dto.Responses.Count == 0)
            {
                return ServiceResult<bool>.Failure("At least one response is required");
            }

            var intake = await _unitOfWork.ServiceIntakeRepository.GetAllQueryable()
                .Include(si => si.Booking)
                .FirstOrDefaultAsync(si => si.Intakeid == dto.IntakeId, ct);

            if (intake == null)
            {
                return ServiceResult<bool>.Failure("Intake not found");
            }

            if (!EditableStates.Contains(intake.Status ?? string.Empty))
            {
                return ServiceResult<bool>.Failure("Intake no longer accepts checklist updates");
            }

            await _unitOfWork.ChecklistRepository.UpsertResponsesAsync(
                dto.IntakeId,
                dto.Responses.Select(r => (r.ItemId, r.Value, r.Note, r.PhotoUrl)),
                ct);

            var now = DateTime.UtcNow;
            if (string.Equals(intake.Status, "CHECKED_IN", StringComparison.OrdinalIgnoreCase))
            {
                intake.Status = "INSPECTING";
            }

            intake.Updatedat = now;

            await _unitOfWork.SaveChangesAsync(ct);

            return ServiceResult<bool>.Success(true, "Checklist responses saved");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error saving responses: {ex.Message}");
        }
    }

    private static string ExtractCategory(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return string.Empty;
        }

        var separatorIndex = code.IndexOf(':');
        return separatorIndex > 0 ? code[..separatorIndex] : code;
    }
}
