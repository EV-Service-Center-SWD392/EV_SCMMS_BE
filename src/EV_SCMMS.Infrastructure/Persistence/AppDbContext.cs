using EV_SCMMS.Core.Domain.Entities;
using EV_SCMMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence;

/// <summary>
/// Application database context with Identity
/// </summary>
public class AppDbContext : IdentityDbContext<Infrastructure.Identity.ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets for business entities
    public DbSet<User> BusinessUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ApplicationUser
        modelBuilder.Entity<Infrastructure.Identity.ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers");
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
        });

        
        // Additional configurations can be added here
    }
}
