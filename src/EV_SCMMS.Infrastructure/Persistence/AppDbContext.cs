using System;
using System.Collections.Generic;
using EV_SCMMS.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Center> Centers { get; set; }

    public virtual DbSet<InventoryTuht> InventoryTuhts { get; set; }

    public virtual DbSet<SparepartTuht> SparepartTuhts { get; set; }

    public virtual DbSet<SparepartforecastTuht> SparepartforecastTuhts { get; set; }

    public virtual DbSet<Sparepartreplenishmentrequest> Sparepartreplenishmentrequests { get; set; }

    public virtual DbSet<SpareparttypeTuht> SpareparttypeTuhts { get; set; }

    public virtual DbSet<SparepartusagehistoryTuht> SparepartusagehistoryTuhts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User Id=postgres.sxevedgnmakrccaqsdfq;Password=Huynhthaitu1;Server=aws-1-ap-southeast-1.pooler.supabase.com;Port=6543;Database=postgres;SSL Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Center>(entity =>
        {
            entity.HasKey(e => e.Centerid).HasName("center_pkey");

            entity.ToTable("center");

            entity.Property(e => e.Centerid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("centerid");
            entity.Property(e => e.Address)
                .HasMaxLength(256)
                .HasColumnName("address");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ACTIVE'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
        });

        modelBuilder.Entity<InventoryTuht>(entity =>
        {
            entity.HasKey(e => e.Inventoryid).HasName("inventory_tuht_pkey");

            entity.ToTable("inventory_tuht");

            entity.Property(e => e.Inventoryid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("inventoryid");
            entity.Property(e => e.Centerid).HasColumnName("centerid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Minimumstocklevel)
                .HasDefaultValue(0)
                .HasColumnName("minimumstocklevel");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(0)
                .HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ACTIVE'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Center).WithMany(p => p.InventoryTuhts)
                .HasForeignKey(d => d.Centerid)
                .HasConstraintName("inventory_tuht_centerid_fkey");
        });

        modelBuilder.Entity<SparepartTuht>(entity =>
        {
            entity.HasKey(e => e.Sparepartid).HasName("sparepart_tuht_pkey");

            entity.ToTable("sparepart_tuht");

            entity.Property(e => e.Sparepartid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("sparepartid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Inventoryid).HasColumnName("inventoryid");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Manufacture)
                .HasMaxLength(256)
                .HasColumnName("manufacture");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ACTIVE'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Typeid).HasColumnName("typeid");
            entity.Property(e => e.Unitprice)
                .HasPrecision(18, 2)
                .HasColumnName("unitprice");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Vehiclemodelid).HasColumnName("vehiclemodelid");

            entity.HasOne(d => d.Inventory).WithMany(p => p.SparepartTuhts)
                .HasForeignKey(d => d.Inventoryid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sparepart_tuht_inventoryid_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.SparepartTuhts)
                .HasForeignKey(d => d.Typeid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sparepart_tuht_typeid_fkey");
        });

        modelBuilder.Entity<SparepartforecastTuht>(entity =>
        {
            entity.HasKey(e => e.Forecastid).HasName("sparepartforecast_tuht_pkey");

            entity.ToTable("sparepartforecast_tuht");

            entity.Property(e => e.Forecastid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("forecastid");
            entity.Property(e => e.Approvedby).HasColumnName("approvedby");
            entity.Property(e => e.Approveddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("approveddate");
            entity.Property(e => e.Centerid).HasColumnName("centerid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Forecastconfidence)
                .HasPrecision(5, 2)
                .HasColumnName("forecastconfidence");
            entity.Property(e => e.Forecastdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("forecastdate");
            entity.Property(e => e.Forecastedby)
                .HasMaxLength(64)
                .HasDefaultValueSql("'AI'::character varying")
                .HasColumnName("forecastedby");
            entity.Property(e => e.Predictedusage)
                .HasDefaultValue(0)
                .HasColumnName("predictedusage");
            entity.Property(e => e.Reorderpoint)
                .HasDefaultValue(0)
                .HasColumnName("reorderpoint");
            entity.Property(e => e.Safetystock)
                .HasDefaultValue(0)
                .HasColumnName("safetystock");
            entity.Property(e => e.Sparepartid).HasColumnName("sparepartid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Center).WithMany(p => p.SparepartforecastTuhts)
                .HasForeignKey(d => d.Centerid)
                .HasConstraintName("sparepartforecast_tuht_centerid_fkey");

            entity.HasOne(d => d.Sparepart).WithMany(p => p.SparepartforecastTuhts)
                .HasForeignKey(d => d.Sparepartid)
                .HasConstraintName("sparepartforecast_tuht_sparepartid_fkey");
        });

        modelBuilder.Entity<Sparepartreplenishmentrequest>(entity =>
        {
            entity.HasKey(e => e.Requestid).HasName("sparepartreplenishmentrequest_pkey");

            entity.ToTable("sparepartreplenishmentrequest");

            entity.Property(e => e.Requestid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("requestid");
            entity.Property(e => e.Approvedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("approvedat");
            entity.Property(e => e.Approvedby).HasColumnName("approvedby");
            entity.Property(e => e.Centerid).HasColumnName("centerid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Forecastid).HasColumnName("forecastid");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.Sparepartid).HasColumnName("sparepartid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Suggestedquantity)
                .HasDefaultValue(0)
                .HasColumnName("suggestedquantity");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Center).WithMany(p => p.Sparepartreplenishmentrequests)
                .HasForeignKey(d => d.Centerid)
                .HasConstraintName("sparepartreplenishmentrequest_centerid_fkey");

            entity.HasOne(d => d.Forecast).WithMany(p => p.Sparepartreplenishmentrequests)
                .HasForeignKey(d => d.Forecastid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sparepartreplenishmentrequest_forecastid_fkey");

            entity.HasOne(d => d.Sparepart).WithMany(p => p.Sparepartreplenishmentrequests)
                .HasForeignKey(d => d.Sparepartid)
                .HasConstraintName("sparepartreplenishmentrequest_sparepartid_fkey");
        });

        modelBuilder.Entity<SpareparttypeTuht>(entity =>
        {
            entity.HasKey(e => e.Typeid).HasName("spareparttype_tuht_pkey");

            entity.ToTable("spareparttype_tuht");

            entity.Property(e => e.Typeid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("typeid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ACTIVE'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
        });

        modelBuilder.Entity<SparepartusagehistoryTuht>(entity =>
        {
            entity.HasKey(e => e.Usageid).HasName("sparepartusagehistory_tuht_pkey");

            entity.ToTable("sparepartusagehistory_tuht");

            entity.Property(e => e.Usageid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("usageid");
            entity.Property(e => e.Centerid).HasColumnName("centerid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Quantityused).HasColumnName("quantityused");
            entity.Property(e => e.Sparepartid).HasColumnName("sparepartid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ACTIVE'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Useddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("useddate");

            entity.HasOne(d => d.Center).WithMany(p => p.SparepartusagehistoryTuhts)
                .HasForeignKey(d => d.Centerid)
                .HasConstraintName("sparepartusagehistory_tuht_centerid_fkey");

            entity.HasOne(d => d.Sparepart).WithMany(p => p.SparepartusagehistoryTuhts)
                .HasForeignKey(d => d.Sparepartid)
                .HasConstraintName("sparepartusagehistory_tuht_sparepartid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
