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

    public virtual DbSet<Assignmentthaontt> Assignmentthaontts { get; set; }

    public virtual DbSet<Bookinghuykt> Bookinghuykts { get; set; }

    public virtual DbSet<Bookingschedule> Bookingschedules { get; set; }

    public virtual DbSet<Bookingstatusloghuykt> Bookingstatusloghuykts { get; set; }

    public virtual DbSet<Centertuantm> Centertuantms { get; set; }

    public virtual DbSet<Certificatetuantm> Certificatetuantms { get; set; }

    public virtual DbSet<Checklistitemthaontt> Checklistitemthaontts { get; set; }

    public virtual DbSet<Checklistresponsethaontt> Checklistresponsethaontts { get; set; }

    public virtual DbSet<InventoryTuht> InventoryTuhts { get; set; }

    public virtual DbSet<Maintenancehistorydungvm> Maintenancehistorydungvms { get; set; }

    public virtual DbSet<Maintenancetaskdungvm> Maintenancetaskdungvms { get; set; }

    public virtual DbSet<Orderservicethaontt> Orderservicethaontts { get; set; }

    public virtual DbSet<Ordersparepart> Orderspareparts { get; set; }

    public virtual DbSet<Orderthaontt> Orderthaontts { get; set; }

    public virtual DbSet<Paymentmethodcuongtq> Paymentcuongtqs { get; set; }

    public virtual DbSet<Transactioncuongtq> Transactioncuongtqs { get; set; }
    public virtual DbSet<Receiptcuongtq> Receiptcuongtqs { get; set; }

    public virtual DbSet<Receiptitemcuongtq> Receiptitemcuongtqs { get; set; }

    public virtual DbSet<Refreshtoken> Refreshtokens { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Serviceintakethaontt> Serviceintakethaontts { get; set; }

    public virtual DbSet<SparepartTuht> SparepartTuhts { get; set; }

    public virtual DbSet<SparepartforecastTuht> SparepartforecastTuhts { get; set; }

    public virtual DbSet<Sparepartreplenishmentrequest> Sparepartreplenishmentrequests { get; set; }

    public virtual DbSet<SpareparttypeTuht> SpareparttypeTuhts { get; set; }

    public virtual DbSet<SparepartusagehistoryTuht> SparepartusagehistoryTuhts { get; set; }

    public virtual DbSet<Useraccount> Useraccounts { get; set; }

    public virtual DbSet<Usercentertuantm> Usercentertuantms { get; set; }

    public virtual DbSet<Usercertificatetuantm> Usercertificatetuantms { get; set; }

    public virtual DbSet<Userrole> Userroles { get; set; }

    public virtual DbSet<Userworkscheduletuantm> Userworkscheduletuantms { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<Vehicleconditiondungvm> Vehicleconditiondungvms { get; set; }

    public virtual DbSet<Vehiclemodel> Vehiclemodels { get; set; }

    public virtual DbSet<Workorderapprovalthaontt> Workorderapprovalthaontts { get; set; }

    public virtual DbSet<Workscheduletuantm> Workscheduletuantms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Assignmentthaontt>(entity =>
        {
            entity.HasKey(e => e.Assignmentid).HasName("assignmentthaontt_pkey");

            entity.ToTable("assignmentthaontt");

            entity.Property(e => e.Assignmentid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("assignmentid");
            entity.Property(e => e.Bookingid).HasColumnName("bookingid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Plannedendutc)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("plannedendutc");
            entity.Property(e => e.Plannedstartutc)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("plannedstartutc");
            // Note: Property 'Note' is ignored as the column doesn't exist in database
            entity.Ignore(e => e.Note);
            entity.Property(e => e.Queueno).HasColumnName("queueno");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Technicianid).HasColumnName("technicianid");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Booking).WithMany(p => p.Assignmentthaontts)
                .HasForeignKey(d => d.Bookingid)
                .HasConstraintName("assignmentthaontt_bookingid_fkey");

            entity.HasOne(d => d.Technician).WithMany(p => p.Assignmentthaontts)
                .HasForeignKey(d => d.Technicianid)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("assignmentthaontt_technicianid_fkey");
        });

        modelBuilder.Entity<Bookinghuykt>(entity =>
        {
            entity.HasKey(e => e.Bookingid).HasName("bookinghuykt_pkey");

            entity.ToTable("bookinghuykt");

            entity.Property(e => e.BookingDate)
                .HasColumnName("bookingdate")
                .HasColumnType("date")
                .HasPrecision(0);

            entity.Property(e => e.Bookingid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("bookingid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Slotid).HasColumnName("slotid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
        .HasDefaultValueSql("CURRENT_TIMESTAMP")
        .HasColumnType("timestamp without time zone")
        .HasColumnName("updatedat");

            entity.Property(e => e.Vehicleid).HasColumnName("vehicleid");

            entity.HasOne(d => d.Customer).WithMany(p => p.Bookinghuykts)
                .HasForeignKey(d => d.Customerid)
                .HasConstraintName("bookinghuykt_customerid_fkey");

            entity.HasOne(d => d.Slot).WithMany(p => p.Bookinghuykts)
                .HasForeignKey(d => d.Slotid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("bookinghuykt_slotid_fkey");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Bookinghuykts)
                .HasForeignKey(d => d.Vehicleid)
                .HasConstraintName("bookinghuykt_vehicleid_fkey");
        });

        modelBuilder.Entity<Bookingschedule>(entity =>
        {
            entity.HasKey(e => e.Slotid).HasName("bookingschedule_pkey");

            entity.ToTable("bookingschedule");

            entity.Property(e => e.Slotid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("slotid");
            entity.Property(e => e.Capacity)
                .HasDefaultValue(1)
                .HasColumnName("capacity");
            entity.Property(e => e.Centerid).HasColumnName("centerid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Endutc)
                    .HasMaxLength(5)
                    .HasColumnName("endutc");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .HasColumnName("note");
            entity.Property(e => e.Startutc)
                .HasMaxLength(5)
                .HasColumnName("startutc");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'OPEN'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(3)
                .HasColumnName("dayofweek");
            entity.Property(e => e.Slot)
                    .HasColumnName("slot");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Center).WithMany(p => p.Bookingschedules)
                .HasForeignKey(d => d.Centerid)
                .HasConstraintName("bookingschedule_centerid_fkey");
        });

        modelBuilder.Entity<Bookingstatusloghuykt>(entity =>
        {
            entity.HasKey(e => e.Logid).HasName("bookingstatusloghuykt_pkey");

            entity.ToTable("bookingstatusloghuykt");

            entity.Property(e => e.Logid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("logid");
            entity.Property(e => e.Bookingid).HasColumnName("bookingid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Isseen)
                .HasDefaultValue(false)
                .HasColumnName("isseen");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Booking).WithMany(p => p.Bookingstatusloghuykts)
                .HasForeignKey(d => d.Bookingid)
                .HasConstraintName("bookingstatusloghuykt_bookingid_fkey");
        });

        modelBuilder.Entity<Centertuantm>(entity =>
        {
            entity.HasKey(e => e.Centerid).HasName("centertuantm_pkey");

            entity.ToTable("centertuantm");

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

        modelBuilder.Entity<Certificatetuantm>(entity =>
        {
            entity.HasKey(e => e.Certificateid).HasName("certificatetuantm_pkey");

            entity.ToTable("certificatetuantm");

            entity.Property(e => e.Certificateid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("certificateid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Description).HasColumnName("description");
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
            entity.Property(e => e.Image)
                .HasColumnName("image");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
        });

        modelBuilder.Entity<Checklistitemthaontt>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("checklistitemthaontt_pkey");

            entity.ToTable("checklistitemthaontt");

            // Unique code (DB script enforces lower(code) unique when not null)
            entity.HasIndex(e => e.Code, "checklistitemthaontt_code_key").IsUnique();
            entity.HasIndex(e => e.Isactive, "ix_checklistitemthaontt_isactive");
            entity.HasIndex(e => e.Status, "ix_checklistitemthaontt_status");

            entity.Property(e => e.Itemid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("itemid");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ACTIVE'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
        });

        modelBuilder.Entity<Checklistresponsethaontt>(entity =>
        {
            entity.HasKey(e => e.Responseid).HasName("checklistresponsethaontt_pkey");

            entity.ToTable("checklistresponsethaontt");

            // Enforce uniqueness: one response per (Intake, Item)
            entity.HasIndex(e => new { e.Intakeid, e.Itemid }).IsUnique();

            entity.Property(e => e.Responseid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("responseid");
            entity.Property(e => e.Comment)
                .HasMaxLength(500)
                .HasColumnName("comment");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Intakeid).HasColumnName("intakeid");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Photourl)
                .HasMaxLength(300)
                .HasColumnName("photourl");
            entity.Property(e => e.Severity).HasColumnName("severity");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'COMPLETED'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Valuebool).HasColumnName("valuebool");
            entity.Property(e => e.Valuenumber)
                .HasPrecision(10, 2)
                .HasColumnName("valuenumber");
            entity.Property(e => e.Valuetext)
                .HasMaxLength(500)
                .HasColumnName("valuetext");

            entity.HasOne(d => d.Intake).WithMany(p => p.Checklistresponsethaontts)
                .HasForeignKey(d => d.Intakeid)
                .HasConstraintName("checklistresponsethaontt_intakeid_fkey");

            entity.HasOne(d => d.Item).WithMany(p => p.Checklistresponsethaontts)
                .HasForeignKey(d => d.Itemid)
                .HasConstraintName("checklistresponsethaontt_itemid_fkey");
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

        modelBuilder.Entity<Maintenancehistorydungvm>(entity =>
        {
            entity.HasKey(e => e.Historyid).HasName("maintenancehistorydungvm_pkey");

            entity.ToTable("maintenancehistorydungvm");

            entity.Property(e => e.Historyid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("historyid");
            entity.Property(e => e.Completeddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("completeddate");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'LOGGED'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Summary).HasColumnName("summary");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Vehicleid).HasColumnName("vehicleid");

            entity.HasOne(d => d.Order).WithMany(p => p.Maintenancehistorydungvms)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("maintenancehistorydungvm_orderid_fkey");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Maintenancehistorydungvms)
                .HasForeignKey(d => d.Vehicleid)
                .HasConstraintName("maintenancehistorydungvm_vehicleid_fkey");
        });

        modelBuilder.Entity<Maintenancetaskdungvm>(entity =>
        {
            entity.HasKey(e => e.Taskid).HasName("maintenancetaskdungvm_pkey");

            entity.ToTable("maintenancetaskdungvm");

            entity.Property(e => e.Taskid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("taskid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Orderdetailid).HasColumnName("orderdetailid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Task)
                .HasMaxLength(256)
                .HasColumnName("task");
            entity.Property(e => e.Technicianid).HasColumnName("technicianid");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Orderdetail).WithMany(p => p.Maintenancetaskdungvms)
                .HasForeignKey(d => d.Orderdetailid)
                .HasConstraintName("maintenancetaskdungvm_orderdetailid_fkey");

            entity.HasOne(d => d.Technician).WithMany(p => p.Maintenancetaskdungvms)
                .HasForeignKey(d => d.Technicianid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("maintenancetaskdungvm_technicianid_fkey");
        });

        modelBuilder.Entity<Orderservicethaontt>(entity =>
        {
            entity.HasKey(e => e.Orderdetailid).HasName("orderservicethaontt_pkey");

            entity.ToTable("orderservicethaontt");

            entity.Property(e => e.Orderdetailid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("orderdetailid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");
            entity.Property(e => e.Serviceid).HasColumnName("serviceid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ADDED'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Unitprice)
                .HasPrecision(18, 2)
                .HasColumnName("unitprice");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Order).WithMany(p => p.Orderservicethaontts)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("orderservicethaontt_orderid_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.Orderservicethaontts)
                .HasForeignKey(d => d.Serviceid)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("orderservicethaontt_serviceid_fkey");
        });

        modelBuilder.Entity<Ordersparepart>(entity =>
        {
            entity.HasKey(e => e.Ordersparepartid).HasName("ordersparepart_pkey");

            entity.ToTable("ordersparepart");

            entity.Property(e => e.Ordersparepartid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("ordersparepartid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Discount)
                .HasPrecision(18, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("discount");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Sparepartid).HasColumnName("sparepartid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ADDED'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Unitprice)
                .HasPrecision(18, 2)
                .HasColumnName("unitprice");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Order).WithMany(p => p.Orderspareparts)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("ordersparepart_orderid_fkey");

            entity.HasOne(d => d.Sparepart).WithMany(p => p.Orderspareparts)
                .HasForeignKey(d => d.Sparepartid)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("ordersparepart_sparepartid_fkey");
        });

        modelBuilder.Entity<Orderthaontt>(entity =>
        {
            entity.HasKey(e => e.Orderid).HasName("orderthaontt_pkey");

            entity.ToTable("orderthaontt");

            entity.HasIndex(e => e.Bookingid, "orderthaontt_bookingid_key").IsUnique();

            entity.Property(e => e.Orderid)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("orderid");
            entity.Property(e => e.Bookingid).HasColumnName("bookingid");
            entity.Property(e => e.Createdat)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("timestamp without time zone")
            .HasColumnName("createdat");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Isactive)
            .HasDefaultValue(true)
            .HasColumnName("isactive");
            entity.Property(e => e.Status)
            .HasMaxLength(50)
            .HasDefaultValueSql("'PENDING_APPROVAL'::character varying")
            .HasColumnName("status");
            entity.Property(e => e.Totalamount)
            .HasPrecision(18, 2)
            .HasDefaultValueSql("0.00")
            .HasColumnName("totalamount");
            entity.Property(e => e.Updatedat)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updatedat");
            entity.Property(e => e.Vehicleid).HasColumnName("vehicleid");

            entity.HasOne(d => d.Booking).WithOne(p => p.Orderthaontt)
            .HasForeignKey<Orderthaontt>(d => d.Bookingid)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("orderthaontt_bookingid_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orderthaontts)
            .HasForeignKey(d => d.Customerid)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("orderthaontt_customerid_fkey");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Orderthaontts)
            .HasForeignKey(d => d.Vehicleid)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("orderthaontt_vehicleid_fkey");
        });

        modelBuilder.Entity<Paymentmethodcuongtq>(entity =>
        {
            entity.HasKey(e => e.Paymentmethodid).HasName("paymentmethodcuongtq_pkey");

            entity.ToTable("paymentmethodcuongtq");

            entity.Property(e => e.Paymentmethodid)
            .HasColumnName("paymentmethodid");
            entity.Property(e => e.Createdat)
            .HasDefaultValueSql("now()")
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
            entity.Property(e => e.Updatedat)
            .HasDefaultValueSql("now()")
            .HasColumnName("updatedat");
        });

        modelBuilder.Entity<Receiptcuongtq>(entity =>
        {
            entity.HasKey(e => e.Receiptid).HasName("receiptcuongtq_pkey");

            entity.ToTable("receiptcuongtq");

            entity.Property(e => e.Receiptid)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("receiptid");
            entity.Property(e => e.Createdat)
            .HasDefaultValueSql("now()")
            .HasColumnName("createdat");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Totalamount)
            .HasPrecision(18, 2)
            .HasColumnName("totalamount");
            entity.Property(e => e.Transactionid).HasColumnName("transactionid");
            entity.Property(e => e.Updatedat)
            .HasDefaultValueSql("now()")
            .HasColumnName("updatedat");

            entity.HasOne(d => d.Customer).WithMany(p => p.Receiptcuongtqs)
            .HasForeignKey(d => d.Customerid)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("receiptcuongtq_customerid_fkey");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Receiptcuongtqs)
            .HasForeignKey(d => d.Transactionid)
            .HasConstraintName("receiptcuongtq_transactionid_fkey");
        });

        modelBuilder.Entity<Receiptitemcuongtq>(entity =>
        {
            entity.HasKey(e => e.Receiptitemid).HasName("receiptitemcuongtq_pkey");

            entity.ToTable("receiptitemcuongtq");

            entity.Property(e => e.Receiptitemid)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("receiptitemid");
            entity.Property(e => e.Createdat)
            .HasDefaultValueSql("now()")
            .HasColumnName("createdat");
            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Itemname)
            .HasMaxLength(256)
            .HasColumnName("itemname");
            entity.Property(e => e.Itemtype)
            .HasMaxLength(50)
            .HasColumnName("itemtype");
            entity.Property(e => e.Linetotal)
            .HasPrecision(18, 2)
            .HasColumnName("linetotal");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Receiptid).HasColumnName("receiptid");
            entity.Property(e => e.Unitprice)
            .HasPrecision(18, 2)
            .HasColumnName("unitprice");
            entity.Property(e => e.Updatedat)
            .HasDefaultValueSql("now()")
            .HasColumnName("updatedat");

            entity.HasOne(d => d.Receipt).WithMany(p => p.Receiptitemcuongtqs)
            .HasForeignKey(d => d.Receiptid)
            .HasConstraintName("receiptitemcuongtq_receiptid_fkey");
        });
        modelBuilder.Entity<Transactioncuongtq>(entity =>
        {
            entity.HasKey(e => e.Transactionid).HasName("transactioncuongtq_pkey");

            entity.ToTable("transactioncuongtq");

            entity.HasIndex(e => e.Orderid, "transactioncuongtq_orderid_key").IsUnique();

            entity.Property(e => e.Transactionid)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("transactionid");
            entity.Property(e => e.Createdat)
            .HasDefaultValueSql("now()")
            .HasColumnName("createdat");
            entity.Property(e => e.Description)
            .HasMaxLength(500)
            .HasColumnName("description");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Paymentmethodid).HasColumnName("paymentmethodid");
            entity.Property(e => e.Reason)
            .HasMaxLength(50)
            .HasColumnName("reason");
            entity.Property(e => e.Paymentid).HasColumnName("paymentid");
            entity.Property(e => e.Staffid).HasColumnName("staffid");
            entity.Property(e => e.Totalamount)
            .HasPrecision(18, 2)
            .HasColumnName("totalamount");
            entity.Property(e => e.Updatedat)
            .HasDefaultValueSql("now()")
            .HasColumnName("updatedat");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Paymentlink).HasColumnName("paymentlink");
            entity.HasOne(d => d.Order).WithOne(p => p.Transactioncuongtq)
            .HasForeignKey<Transactioncuongtq>(d => d.Orderid)
            .HasConstraintName("transactioncuongtq_orderid_fkey");

            entity.HasOne(d => d.Paymentmethod).WithMany(p => p.Transactioncuongtqs)
            .HasForeignKey(d => d.Paymentmethodid)
            .HasConstraintName("transactioncuongtq_paymentmethodid_fkey");

            entity.HasOne(d => d.Staff).WithMany(p => p.Transactioncuongtqs)
            .HasForeignKey(d => d.Staffid)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("transactioncuongtq_staffid_fkey");
        });

        modelBuilder.Entity<Refreshtoken>(entity =>
            {
                entity.HasKey(e => e.Tokenid).HasName("refreshtoken_pkey");

                entity.ToTable("refreshtoken");

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
                entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
                entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'VALID'::character varying")
                .HasColumnName("status");
                entity.Property(e => e.Token)
                .HasMaxLength(512)
                .HasColumnName("token");
                entity.Property(e => e.Userid).HasColumnName("userid");

                entity.HasOne(d => d.User).WithMany(p => p.Refreshtokens)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("refreshtoken_userid_fkey");
            });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Serviceid).HasName("service_pkey");

            entity.ToTable("service");

            entity.Property(e => e.Serviceid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("serviceid");
            entity.Property(e => e.Baseprice)
                .HasPrecision(18, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("baseprice");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Durationminutes).HasColumnName("durationminutes");
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

        modelBuilder.Entity<Serviceintakethaontt>(entity =>
        {
            entity.HasKey(e => e.Intakeid).HasName("serviceintakethaontt_pkey");

            entity.ToTable("serviceintakethaontt");

            entity.HasIndex(e => e.Bookingid, "serviceintakethaontt_bookingid_key").IsUnique();

            entity.Property(e => e.Intakeid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("intakeid");
            entity.Property(e => e.CheckedInBy).HasColumnName("advisorid");
            entity.Property(e => e.Batterysoc).HasColumnName("batterysoc");
            entity.Property(e => e.Bookingid).HasColumnName("bookingid");
            entity.Property(e => e.Checkintimeutc)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("checkintimeutc");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Notes).HasColumnName("notes");
            //entity.Property(e => e.IntakeResponseNote).HasColumnName("intakeresponsenote");
            entity.Property(e => e.Odometerkm).HasColumnName("odometerkm");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'CHECKED_IN'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Advisor).WithMany(p => p.Serviceintakethaontts)
                .HasForeignKey(d => d.CheckedInBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("serviceintakethaontt_advisorid_fkey");

            entity.HasOne(d => d.Booking).WithOne(p => p.Serviceintakethaontt)
                .HasForeignKey<Serviceintakethaontt>(d => d.Bookingid)
                .HasConstraintName("serviceintakethaontt_bookingid_fkey");
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

            entity.HasOne(d => d.Vehiclemodel).WithMany(p => p.SparepartTuhts)
                .HasForeignKey(d => d.Vehiclemodelid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sparepart_tuht_vehiclemodelid_fkey");
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
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
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
            entity.Property(e => e.Approvedby).HasColumnName("approvedby");
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
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
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
            entity.Property(e => e.Ordersparepartid).HasColumnName("ordersparepartid");
            entity.Property(e => e.Quantityused).HasColumnName("quantityused");
            entity.Property(e => e.Sparepartid).HasColumnName("sparepartid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'USED'::character varying")
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

            entity.HasOne(d => d.Ordersparepart).WithMany(p => p.SparepartusagehistoryTuhts)
                .HasForeignKey(d => d.Ordersparepartid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sparepartusagehistory_tuht_ordersparepartid_fkey");

            entity.HasOne(d => d.Sparepart).WithMany(p => p.SparepartusagehistoryTuhts)
                .HasForeignKey(d => d.Sparepartid)
                .HasConstraintName("sparepartusagehistory_tuht_sparepartid_fkey");
        });

        modelBuilder.Entity<Useraccount>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("useraccount_pkey");

            entity.ToTable("useraccount");

            entity.HasIndex(e => e.Email, "useraccount_email_key").IsUnique();

            entity.Property(e => e.Userid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("userid");
            entity.Property(e => e.Address)
                .HasMaxLength(256)
                .HasColumnName("address");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Lastname)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .HasColumnName("password");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(15)
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

            entity.HasOne(d => d.Role).WithMany(p => p.Useraccounts)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("useraccount_roleid_fkey");
        });

        modelBuilder.Entity<Usercentertuantm>(entity =>
        {
            entity.HasKey(e => e.Usercenterid).HasName("usercentertuantm_pkey");

            entity.ToTable("usercentertuantm");

            entity.HasIndex(e => new { e.Userid, e.Centerid }, "usercentertuantm_userid_centerid_key").IsUnique();

            entity.Property(e => e.Usercenterid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("usercenterid");
            entity.Property(e => e.Centerid).HasColumnName("centerid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'WORKING'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Worksince)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("worksince");

            entity.HasOne(d => d.Center).WithMany(p => p.Usercentertuantms)
                .HasForeignKey(d => d.Centerid)
                .HasConstraintName("usercentertuantm_centerid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Usercentertuantms)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("usercentertuantm_userid_fkey");
        });

        modelBuilder.Entity<Usercertificatetuantm>(entity =>
        {
            entity.HasKey(e => e.Usercertificateid).HasName("usercertificatetuantm_pkey");

            entity.ToTable("usercertificatetuantm");

            entity.HasIndex(e => new { e.Userid, e.Certificateid }, "usercertificatetuantm_userid_certificateid_key").IsUnique();

            entity.Property(e => e.Usercertificateid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("usercertificateid");
            entity.Property(e => e.Certificateid).HasColumnName("certificateid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'VALID'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Certificate).WithMany(p => p.Usercertificatetuantms)
                .HasForeignKey(d => d.Certificateid)
                .HasConstraintName("usercertificatetuantm_certificateid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Usercertificatetuantms)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("usercertificatetuantm_userid_fkey");
        });

        modelBuilder.Entity<Userrole>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("userrole_pkey");

            entity.ToTable("userrole");

            entity.HasIndex(e => e.Name, "userrole_name_key").IsUnique();

            entity.Property(e => e.Roleid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("roleid");
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
                .HasMaxLength(128)
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

        modelBuilder.Entity<Userworkscheduletuantm>(entity =>
        {
            entity.HasKey(e => e.Userworkscheduleid).HasName("userworkscheduletuantm_pkey");

            entity.ToTable("userworkscheduletuantm");

            entity.HasIndex(e => new { e.Userid, e.Workscheduleid }, "userworkscheduletuantm_userid_workscheduleid_key").IsUnique();

            entity.Property(e => e.Userworkscheduleid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("userworkscheduleid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ASSIGNED'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Workscheduleid).HasColumnName("workscheduleid");

            entity.HasOne(d => d.User).WithMany(p => p.Userworkscheduletuantms)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("userworkscheduletuantm_userid_fkey");

            entity.HasOne(d => d.Workschedule).WithMany(p => p.Userworkscheduletuantms)
                .HasForeignKey(d => d.Workscheduleid)
                .HasConstraintName("userworkscheduletuantm_workscheduleid_fkey");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Vehicleid).HasName("vehicle_pkey");

            entity.ToTable("vehicle");

            entity.HasIndex(e => e.Licenseplate, "vehicle_licenseplate_key").IsUnique();

            entity.Property(e => e.Vehicleid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("vehicleid");
            entity.Property(e => e.Color)
                .HasMaxLength(64)
                .HasColumnName("color");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Licenseplate)
                .HasMaxLength(50)
                .HasColumnName("licenseplate");
            entity.Property(e => e.Modelid).HasColumnName("modelid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ACTIVE'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.Customer).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.Customerid)
                .HasConstraintName("vehicle_customerid_fkey");

            entity.HasOne(d => d.Model).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.Modelid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("vehicle_modelid_fkey");
        });

        modelBuilder.Entity<Vehicleconditiondungvm>(entity =>
        {
            entity.HasKey(e => e.Conditionid).HasName("vehicleconditiondungvm_pkey");

            entity.ToTable("vehicleconditiondungvm");

            entity.Property(e => e.Conditionid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("conditionid");
            entity.Property(e => e.Batteryhealth).HasColumnName("batteryhealth");
            entity.Property(e => e.Bodystatus)
                .HasMaxLength(256)
                .HasColumnName("bodystatus");
            entity.Property(e => e.Brakestatus)
                .HasMaxLength(256)
                .HasColumnName("brakestatus");
            entity.Property(e => e.Condition).HasColumnName("condition");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Lastmaintenance)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lastmaintenance");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'RECORDED'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Tirepressure).HasColumnName("tirepressure");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Vehicleid).HasColumnName("vehicleid");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Vehicleconditiondungvms)
                .HasForeignKey(d => d.Vehicleid)
                .HasConstraintName("vehicleconditiondungvm_vehicleid_fkey");
        });

        modelBuilder.Entity<Vehiclemodel>(entity =>
        {
            entity.HasKey(e => e.Modelid).HasName("vehiclemodel_pkey");

            entity.ToTable("vehiclemodel");

            entity.Property(e => e.Modelid).HasColumnName("modelid");
            entity.Property(e => e.Brand)
                .HasMaxLength(128)
                .HasColumnName("brand");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Enginetype)
                .HasMaxLength(128)
                .HasColumnName("enginetype");
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

        modelBuilder.Entity<Workorderapprovalthaontt>(entity =>
        {
            entity.HasKey(e => e.WoaId).HasName("workorderapprovalthaontt_pkey");

            entity.ToTable("workorderapprovalthaontt");

            entity.Property(e => e.WoaId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("woa_id");
            entity.Property(e => e.Approvedby).HasColumnName("approvedby");
            entity.Property(e => e.Approvedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("approvedat");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Method)
                .HasMaxLength(20)
                .HasColumnName("method");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .HasColumnName("note");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'AWAITING'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Order).WithMany(p => p.Workorderapprovalthaontts)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("workorderapprovalthaontt_orderid_fkey");
        });

        modelBuilder.Entity<Workscheduletuantm>(entity =>
        {
            entity.HasKey(e => e.Workscheduleid).HasName("workscheduletuantm_pkey");

            entity.ToTable("workscheduletuantm");

            entity.Property(e => e.Workscheduleid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("workscheduleid");
            entity.Property(e => e.Centerid).HasColumnName("centerid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Endtime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("endtime");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Starttime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("starttime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'PLANNED'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Center).WithMany(p => p.Workscheduletuantms)
                .HasForeignKey(d => d.Centerid)
                .HasConstraintName("workscheduletuantm_centerid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
