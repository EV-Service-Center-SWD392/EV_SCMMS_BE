using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

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

  public virtual DbSet<User> Appusers { get; set; }

  public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

  public virtual DbSet<ActiveRefreshToken> ActiveRefreshTokens { get; set; }

  public virtual DbSet<Center> Centers { get; set; }

  public virtual DbSet<InventoryTuht> InventoryTuhts { get; set; }

  public virtual DbSet<Role> Roles { get; set; }

  public virtual DbSet<SparepartTuht> SparepartTuhts { get; set; }

  public virtual DbSet<SparepartforecastTuht> SparepartforecastTuhts { get; set; }

  public virtual DbSet<Sparepartreplenishmentrequest> Sparepartreplenishmentrequests { get; set; }

  public virtual DbSet<SpareparttypeTuht> SpareparttypeTuhts { get; set; }

  public virtual DbSet<SparepartusagehistoryTuht> SparepartusagehistoryTuhts { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
      => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ev_scmms_db;Username=postgres;Password=12345A");

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasPostgresExtension("uuid-ossp");

    modelBuilder.Entity<User>(entity =>
    {
      entity.ToTable("appuser");
      entity.HasKey(e => e.Userid).HasName("appuser_pkey");

      entity.ToTable("appuser");

      entity.HasIndex(e => e.Email, "appuser_email_key").IsUnique();

      entity.HasIndex(e => e.Phonenumber, "appuser_phonenumber_key").IsUnique();

      entity.Property(e => e.Userid)
              .HasDefaultValueSql("uuid_generate_v4()")
              .HasColumnName("userid");
      entity.Property(e => e.Address)
              .HasMaxLength(256)
              .HasColumnName("address");
      entity.Property(e => e.Birthday).HasColumnName("birthday");
      entity.Property(e => e.Createdat)
              .HasDefaultValueSql("CURRENT_TIMESTAMP")
              .HasColumnType("timestamp without time zone")
              .HasColumnName("createdat");
      entity.Property(e => e.Email)
              .HasMaxLength(256)
              .HasColumnName("email");
      entity.Property(e => e.Firstname)
              .HasMaxLength(128)
              .HasColumnName("firstname");
      entity.Property(e => e.Isactive)
              .HasDefaultValue(true)
              .HasColumnName("isactive");
      entity.Property(e => e.Lastname)
              .HasMaxLength(128)
              .HasColumnName("lastname");
      entity.Property(e => e.Password)
              .HasMaxLength(256)
              .HasColumnName("password");
      entity.Property(e => e.Phonenumber)
              .HasMaxLength(32)
              .HasColumnName("phonenumber");
      entity.Property(e => e.Roleid).HasColumnName("roleid");
      entity.Property(e => e.Status)
              .HasMaxLength(50)
              .HasDefaultValueSql("'ACTIVE'::character varying")
              .HasColumnName("status");
      entity.Property(e => e.Updatedat)
              .HasDefaultValueSql("CURRENT_TIMESTAMP")
              .HasColumnType("timestamp without time zone")
              .HasColumnName("updatedat");

      entity.HasOne(d => d.Role).WithMany(p => p.Appusers)
              .HasForeignKey(d => d.Roleid)
              .OnDelete(DeleteBehavior.Restrict)
              .HasConstraintName("appuser_roleid_fkey");
    });
    modelBuilder.Entity<ActiveRefreshToken>(entity =>
    {
      entity
          .HasNoKey()
          .ToView("activerefreshtokens");

      entity.Property(e => e.Createdat)
          .HasColumnType("timestamp without time zone")
          .HasColumnName("createdat");
      entity.Property(e => e.Expiresat)
          .HasColumnType("timestamp without time zone")
          .HasColumnName("expiresat");
      entity.Property(e => e.Token)
          .HasMaxLength(512)
          .HasColumnName("token");
      entity.Property(e => e.Tokenid).HasColumnName("tokenid");
      entity.Property(e => e.Userid).HasColumnName("userid");
    });
    modelBuilder.Entity<RefreshToken>(entity =>
    {
      entity.HasKey(e => e.Tokenid).HasName("refreshtoken_pkey");

      entity.ToTable("refreshtoken");

      entity.HasIndex(e => e.Expiresat, "idx_refreshtoken_expiresat");

      entity.HasIndex(e => e.Token, "idx_refreshtoken_token");

      entity.HasIndex(e => e.Userid, "idx_refreshtoken_userid");

      entity.HasIndex(e => e.Token, "refreshtoken_token_key").IsUnique();

      entity.Property(e => e.Tokenid)
          .HasDefaultValueSql("uuid_generate_v4()")
          .HasColumnName("tokenid");
      entity.Property(e => e.Createdat)
          .HasDefaultValueSql("CURRENT_TIMESTAMP")
          .HasColumnType("timestamp without time zone")
          .HasColumnName("createdat");
      entity.Property(e => e.Expiresat)
          .HasColumnType("timestamp without time zone")
          .HasColumnName("expiresat");
      entity.Property(e => e.Token)
          .HasMaxLength(512)
          .HasColumnName("token");
      entity.Property(e => e.Userid).HasColumnName("userid");

      entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
          .HasForeignKey(d => d.Userid)
          .HasConstraintName("refreshtoken_userid_fkey");
    });
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

    modelBuilder.Entity<Role>(entity =>
    {
      entity.HasKey(e => e.Roleid).HasName("role_pkey");

      entity.ToTable("role");

      entity.Property(e => e.Roleid)
              .HasDefaultValueSql("uuid_generate_v4()")
              .HasColumnName("roleid");
      entity.Property(e => e.Description)
              .HasMaxLength(500)
              .HasColumnName("description");
      entity.Property(e => e.Name)
              .HasMaxLength(128)
              .HasColumnName("name");
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
