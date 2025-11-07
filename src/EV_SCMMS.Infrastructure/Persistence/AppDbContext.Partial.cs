using Microsoft.EntityFrameworkCore;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Persistence;

/// <summary>
/// Partial class for additional DbContext configuration
/// </summary>
public partial class AppDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // Ignore the Note property in Assignmentthaontt as it doesn't exist in the database
        modelBuilder.Entity<Assignmentthaontt>()
            .Ignore(e => e.Note);
    }
}
