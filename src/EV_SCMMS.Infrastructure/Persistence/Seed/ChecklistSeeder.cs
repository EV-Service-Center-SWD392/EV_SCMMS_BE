using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Seed;

public static class ChecklistSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken ct = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var hasAny = await context.Checklistitemthaontts.AsNoTracking().AnyAsync(ct);
        if (hasAny) return;

        var now = DateTime.UtcNow;
        var items = new List<Checklistitemthaontt>
        {
            new()
            {
                Code = "GENERAL:Exterior",
                Name = "Exterior Check",
                Type = 1, // boolean
                Unit = null,
                Status = "ACTIVE",
                Isactive = true,
                Createdat = now,
                Updatedat = now
            },
            new()
            {
                Code = "GENERAL:Interior",
                Name = "Interior Check",
                Type = 1, // boolean
                Unit = null,
                Status = "ACTIVE",
                Isactive = true,
                Createdat = now,
                Updatedat = now
            },
            new()
            {
                Code = "ELECTRICAL:BatteryVoltage",
                Name = "Battery Voltage",
                Type = 2, // number
                Unit = "V",
                Status = "ACTIVE",
                Isactive = true,
                Createdat = now,
                Updatedat = now
            },
            new()
            {
                Code = "BRAKE:PadThickness",
                Name = "Brake Pad Thickness",
                Type = 2, // number
                Unit = "mm",
                Status = "ACTIVE",
                Isactive = true,
                Createdat = now,
                Updatedat = now
            },
            new()
            {
                Code = "TIRE:PressureFL",
                Name = "Tire Pressure Front Left",
                Type = 2, // number
                Unit = "psi",
                Status = "ACTIVE",
                Isactive = true,
                Createdat = now,
                Updatedat = now
            },
            new()
            {
                Code = "TIRE:ConditionSeverity",
                Name = "Tire Condition Severity",
                Type = 3, // severity (short)
                Unit = null,
                Status = "ACTIVE",
                Isactive = true,
                Createdat = now,
                Updatedat = now
            },
            new()
            {
                Code = "NOTES:General",
                Name = "General Notes",
                Type = null, // free text
                Unit = null,
                Status = "ACTIVE",
                Isactive = true,
                Createdat = now,
                Updatedat = now
            }
        };

        await context.Checklistitemthaontts.AddRangeAsync(items, ct);
        await context.SaveChangesAsync(ct);
    }
}
