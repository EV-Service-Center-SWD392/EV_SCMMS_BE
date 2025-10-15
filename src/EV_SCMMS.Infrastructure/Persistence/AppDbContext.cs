using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

    public virtual DbSet<AssignmentThaoNtt> AssignmentThaoNtts { get; set; }

    public virtual DbSet<BookingScheduleThaoNtt> BookingScheduleThaoNtts { get; set; }

    public virtual DbSet<BookingThaoNtt> BookingThaoNtts { get; set; }

    public virtual DbSet<Center> Centers { get; set; }

    public virtual DbSet<ChecklistItemThaoNtt> ChecklistItemThaoNtts { get; set; }

    public virtual DbSet<ChecklistResponseThaoNtt> ChecklistResponseThaoNtts { get; set; }

    public virtual DbSet<InventoryTuHt> InventoryTuHts { get; set; }

    public virtual DbSet<MaintenanceHistoryDungVm> MaintenanceHistoryDungVms { get; set; }

    public virtual DbSet<MaintenanceTaskDungVm> MaintenanceTaskDungVms { get; set; }

    public virtual DbSet<OrderServiceThaoNtt> OrderServiceThaoNtts { get; set; }

    public virtual DbSet<OrderThaoNtt> OrderThaoNtts { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceIntakeThaoNtt> ServiceIntakeThaoNtts { get; set; }

    public virtual DbSet<SparePartForecastTuHt> SparePartForecastTuHts { get; set; }

    public virtual DbSet<SparePartReplenishmentRequest> SparePartReplenishmentRequests { get; set; }

    public virtual DbSet<SparePartTuHt> SparePartTuHts { get; set; }

    public virtual DbSet<SparePartTypeTuHt> SparePartTypeTuHts { get; set; }

    public virtual DbSet<SparePartUsageHistoryTuHt> SparePartUsageHistoryTuHts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleConditionDungVm> VehicleConditionDungVms { get; set; }

    public virtual DbSet<WorkOrderApprovalThaoNtt> WorkOrderApprovalThaoNtts { get; set; }

    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            //.AddJsonFile("appsettings.Development.json")
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(GetConnectionString("DefaultConnection"))
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<AssignmentThaoNtt>(entity =>
        {
            entity.HasKey(e => e.AssThaoNttid).HasName("AssignmentThaoNTT_pkey");

            entity.ToTable("AssignmentThaoNTT");

            entity.HasIndex(e => e.BookingId, "idx_assign_booking");

            entity.HasIndex(e => e.TechnicianId, "idx_assign_tech");

            entity.HasIndex(e => e.BookingId, "idx_thaontt_assign_booking");

            entity.HasIndex(e => e.Status, "idx_thaontt_assign_status");

            entity.HasIndex(e => e.TechnicianId, "idx_thaontt_assign_tech");

            entity.Property(e => e.AssThaoNttid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("AssThaoNTTId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.PlannedEndUtc).HasColumnType("timestamp without time zone");
            entity.Property(e => e.PlannedStartUtc).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Booking).WithMany(p => p.AssignmentThaoNtts)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("AssignmentThaoNTT_BookingId_fkey");

            entity.HasOne(d => d.Technician).WithMany(p => p.AssignmentThaoNtts)
                .HasForeignKey(d => d.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("AssignmentThaoNTT_TechnicianId_fkey");
        });

        modelBuilder.Entity<BookingScheduleThaoNtt>(entity =>
        {
            entity.HasKey(e => e.BsthaoNttid).HasName("BookingScheduleThaoNTT_pkey");

            entity.ToTable("BookingScheduleThaoNTT");

            entity.HasIndex(e => new { e.CenterId, e.StartUtc }, "idx_sched_center_start");

            entity.HasIndex(e => e.CenterId, "idx_thaontt_sched_center");

            entity.HasIndex(e => new { e.StartUtc, e.EndUtc }, "idx_thaontt_sched_time");

            entity.HasIndex(e => new { e.CenterId, e.StartUtc, e.EndUtc }, "uq_schedule_slot").IsUnique();

            entity.Property(e => e.BsthaoNttid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("BSThaoNTTId");
            entity.Property(e => e.Capacity).HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.EndUtc).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.StartUtc).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Status).HasDefaultValue((short)0);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Center).WithMany(p => p.BookingScheduleThaoNtts)
                .HasForeignKey(d => d.CenterId)
                .HasConstraintName("BookingScheduleThaoNTT_CenterId_fkey");
        });

        modelBuilder.Entity<BookingThaoNtt>(entity =>
        {
            entity.HasKey(e => e.BookingThaoNttid).HasName("BookingThaoNTT_pkey");

            entity.ToTable("BookingThaoNTT");

            entity.HasIndex(e => e.CustomerId, "idx_booking_customer");

            entity.HasIndex(e => e.VehicleId, "idx_booking_vehicle");

            entity.HasIndex(e => e.CustomerId, "idx_thaontt_booking_customer");

            entity.HasIndex(e => e.BookingScheduleId, "idx_thaontt_booking_schedule");

            entity.HasIndex(e => e.Status, "idx_thaontt_booking_status");

            entity.HasIndex(e => e.VehicleId, "idx_thaontt_booking_vehicle");

            entity.Property(e => e.BookingThaoNttid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("BookingThaoNTTId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.Status).HasDefaultValue((short)0);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.BookingSchedule).WithMany(p => p.BookingThaoNtts)
                .HasForeignKey(d => d.BookingScheduleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("BookingThaoNTT_BookingScheduleId_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.BookingThaoNtts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("BookingThaoNTT_CustomerId_fkey");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.BookingThaoNtts)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("BookingThaoNTT_VehicleId_fkey");
        });

        modelBuilder.Entity<Center>(entity =>
        {
            entity.HasKey(e => e.CenterId).HasName("Center_pkey");

            entity.ToTable("Center");

            entity.Property(e => e.CenterId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("CenterID");
            entity.Property(e => e.Address).HasMaxLength(256);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ACTIVE'::character varying");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<ChecklistItemThaoNtt>(entity =>
        {
            entity.HasKey(e => e.ClithaoNttid).HasName("ChecklistItemThaoNTT_pkey");

            entity.ToTable("ChecklistItemThaoNTT");

            entity.HasIndex(e => e.Code, "ChecklistItemThaoNTT_Code_key").IsUnique();

            entity.HasIndex(e => e.Code, "idx_thaontt_chkitem_code");

            entity.HasIndex(e => e.Type, "idx_thaontt_chkitem_type");

            entity.Property(e => e.ClithaoNttid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("CLIThaoNTTId");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Type).HasDefaultValue((short)0);
            entity.Property(e => e.Unit).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<ChecklistResponseThaoNtt>(entity =>
        {
            entity.HasKey(e => e.CrthaoNttid).HasName("ChecklistResponseThaoNTT_pkey");

            entity.ToTable("ChecklistResponseThaoNTT");

            entity.HasIndex(e => e.IntakeId, "idx_resp_intake");

            entity.HasIndex(e => e.ItemId, "idx_resp_item");

            entity.HasIndex(e => e.IntakeId, "idx_thaontt_chkresp_intake");

            entity.HasIndex(e => e.ItemId, "idx_thaontt_chkresp_item");

            entity.HasIndex(e => e.Severity, "idx_thaontt_chkresp_severity");

            entity.HasIndex(e => new { e.IntakeId, e.ItemId }, "uq_chkresp_intake_item").IsUnique();

            entity.Property(e => e.CrthaoNttid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("CRThaoNTTId");
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.PhotoUrl).HasMaxLength(300);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.ValueNumber).HasPrecision(10, 2);
            entity.Property(e => e.ValueText).HasMaxLength(500);

            entity.HasOne(d => d.Intake).WithMany(p => p.ChecklistResponseThaoNtts)
                .HasForeignKey(d => d.IntakeId)
                .HasConstraintName("ChecklistResponseThaoNTT_IntakeId_fkey");

            entity.HasOne(d => d.Item).WithMany(p => p.ChecklistResponseThaoNtts)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("ChecklistResponseThaoNTT_ItemId_fkey");
        });

        modelBuilder.Entity<InventoryTuHt>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("Inventory_TuHT_pkey");

            entity.ToTable("Inventory_TuHT");

            entity.Property(e => e.InventoryId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("InventoryID");
            entity.Property(e => e.CenterId).HasColumnName("CenterID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MinimumStockLevel).HasDefaultValue(0);
            entity.Property(e => e.Quantity).HasDefaultValue(0);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ACTIVE'::character varying");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Center).WithMany(p => p.InventoryTuHts)
                .HasForeignKey(d => d.CenterId)
                .HasConstraintName("Inventory_TuHT_CenterID_fkey");
        });

        modelBuilder.Entity<MaintenanceHistoryDungVm>(entity =>
        {
            entity.HasKey(e => e.MaintenanceHistoryDungVmid).HasName("MaintenanceHistoryDungVM_pkey");

            entity.ToTable("MaintenanceHistoryDungVM");

            entity.HasIndex(e => e.OrderId, "idx_mhdungvm_order");

            entity.HasIndex(e => e.VehicleId, "idx_mhdungvm_vehicle");

            entity.Property(e => e.MaintenanceHistoryDungVmid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("MaintenanceHistoryDungVMId");
            entity.Property(e => e.CompletedDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Summary).HasMaxLength(1000);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Order).WithMany(p => p.MaintenanceHistoryDungVms)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("MaintenanceHistoryDungVM_OrderId_fkey");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.MaintenanceHistoryDungVms)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("MaintenanceHistoryDungVM_VehicleId_fkey");
        });

        modelBuilder.Entity<MaintenanceTaskDungVm>(entity =>
        {
            entity.HasKey(e => e.MaintenanceTaskDungVmid).HasName("MaintenanceTaskDungVM_pkey");

            entity.ToTable("MaintenanceTaskDungVM");

            entity.HasIndex(e => e.OrderServiceId, "idx_mtdungvm_order_service");

            entity.HasIndex(e => e.Status, "idx_mtdungvm_status");

            entity.HasIndex(e => e.TechnicianId, "idx_mtdungvm_technician");

            entity.Property(e => e.MaintenanceTaskDungVmid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("MaintenanceTaskDungVMId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Status).HasMaxLength(100);
            entity.Property(e => e.Task).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.OrderService).WithMany(p => p.MaintenanceTaskDungVms)
                .HasForeignKey(d => d.OrderServiceId)
                .HasConstraintName("MaintenanceTaskDungVM_OrderServiceId_fkey");

            entity.HasOne(d => d.Technician).WithMany(p => p.MaintenanceTaskDungVms)
                .HasForeignKey(d => d.TechnicianId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("MaintenanceTaskDungVM_TechnicianId_fkey");
        });

        modelBuilder.Entity<OrderServiceThaoNtt>(entity =>
        {
            entity.HasKey(e => e.OsthaoNttid).HasName("OrderServiceThaoNTT_pkey");

            entity.ToTable("OrderServiceThaoNTT");

            entity.HasIndex(e => e.OrderId, "idx_os_order");

            entity.HasIndex(e => e.OrderId, "idx_thaontt_os_order");

            entity.HasIndex(e => e.ServiceId, "idx_thaontt_os_service");

            entity.Property(e => e.OsthaoNttid).HasColumnName("OSThaoNTTId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderServiceThaoNtts)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("OrderServiceThaoNTT_OrderId_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.OrderServiceThaoNtts)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("OrderServiceThaoNTT_ServiceId_fkey");
        });

        modelBuilder.Entity<OrderThaoNtt>(entity =>
        {
            entity.HasKey(e => e.OrderThaoNttid).HasName("OrderThaoNTT_pkey");

            entity.ToTable("OrderThaoNTT");

            entity.HasIndex(e => e.BookingId, "idx_order_booking");

            entity.HasIndex(e => e.CustomerId, "idx_order_customer");

            entity.HasIndex(e => e.BookingId, "idx_thaontt_order_booking");

            entity.HasIndex(e => e.CustomerId, "idx_thaontt_order_customer");

            entity.HasIndex(e => e.Status, "idx_thaontt_order_status");

            entity.HasIndex(e => e.VehicleId, "idx_thaontt_order_vehicle");

            entity.Property(e => e.OrderThaoNttid).HasColumnName("OrderThaoNTTId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Status).HasDefaultValue((short)0);
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2)
                .HasDefaultValueSql("0");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Booking).WithMany(p => p.OrderThaoNtts)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("OrderThaoNTT_BookingId_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderThaoNtts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("OrderThaoNTT_CustomerId_fkey");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.OrderThaoNtts)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("OrderThaoNTT_VehicleId_fkey");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Service_pkey");

            entity.ToTable("Service");

            entity.Property(e => e.BasePrice).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<ServiceIntakeThaoNtt>(entity =>
        {
            entity.HasKey(e => e.SithaoNttid).HasName("ServiceIntakeThaoNTT_pkey");

            entity.ToTable("ServiceIntakeThaoNTT");

            entity.HasIndex(e => e.BookingId, "idx_intake_booking");

            entity.HasIndex(e => e.AdvisorId, "idx_thaontt_intake_advisor");

            entity.HasIndex(e => e.BookingId, "idx_thaontt_intake_booking");

            entity.Property(e => e.SithaoNttid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("SIThaoNTTId");
            entity.Property(e => e.CheckinTimeUtc).HasColumnType("timestamp without time zone");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Advisor).WithMany(p => p.ServiceIntakeThaoNtts)
                .HasForeignKey(d => d.AdvisorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("ServiceIntakeThaoNTT_AdvisorId_fkey");

            entity.HasOne(d => d.Booking).WithMany(p => p.ServiceIntakeThaoNtts)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("ServiceIntakeThaoNTT_BookingId_fkey");
        });

        modelBuilder.Entity<SparePartForecastTuHt>(entity =>
        {
            entity.HasKey(e => e.ForecastId).HasName("SparePartForecast_TuHT_pkey");

            entity.ToTable("SparePartForecast_TuHT");

            entity.Property(e => e.ForecastId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("ForecastID");
            entity.Property(e => e.ApprovedDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.CenterId).HasColumnName("CenterID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.ForecastConfidence).HasPrecision(5, 2);
            entity.Property(e => e.ForecastDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.ForecastedBy)
                .HasMaxLength(64)
                .HasDefaultValueSql("'AI'::character varying");
            entity.Property(e => e.PredictedUsage).HasDefaultValue(0);
            entity.Property(e => e.ReorderPoint).HasDefaultValue(0);
            entity.Property(e => e.SafetyStock).HasDefaultValue(0);
            entity.Property(e => e.SparePartId).HasColumnName("SparePartID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'PENDING'::character varying");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Center).WithMany(p => p.SparePartForecastTuHts)
                .HasForeignKey(d => d.CenterId)
                .HasConstraintName("SparePartForecast_TuHT_CenterID_fkey");

            entity.HasOne(d => d.SparePart).WithMany(p => p.SparePartForecastTuHts)
                .HasForeignKey(d => d.SparePartId)
                .HasConstraintName("SparePartForecast_TuHT_SparePartID_fkey");
        });

        modelBuilder.Entity<SparePartReplenishmentRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("SparePartReplenishmentRequest_pkey");

            entity.ToTable("SparePartReplenishmentRequest");

            entity.Property(e => e.RequestId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("RequestID");
            entity.Property(e => e.ApprovedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.CenterId).HasColumnName("CenterID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.ForecastId).HasColumnName("ForecastID");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.SparePartId).HasColumnName("SparePartID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'PENDING'::character varying");
            entity.Property(e => e.SuggestedQuantity).HasDefaultValue(0);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Center).WithMany(p => p.SparePartReplenishmentRequests)
                .HasForeignKey(d => d.CenterId)
                .HasConstraintName("SparePartReplenishmentRequest_CenterID_fkey");

            entity.HasOne(d => d.Forecast).WithMany(p => p.SparePartReplenishmentRequests)
                .HasForeignKey(d => d.ForecastId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SparePartReplenishmentRequest_ForecastID_fkey");

            entity.HasOne(d => d.SparePart).WithMany(p => p.SparePartReplenishmentRequests)
                .HasForeignKey(d => d.SparePartId)
                .HasConstraintName("SparePartReplenishmentRequest_SparePartID_fkey");
        });

        modelBuilder.Entity<SparePartTuHt>(entity =>
        {
            entity.HasKey(e => e.SparePartId).HasName("SparePart_TuHT_pkey");

            entity.ToTable("SparePart_TuHT");

            entity.Property(e => e.SparePartId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("SparePartID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.InventoryId).HasColumnName("InventoryID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Manufacture).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ACTIVE'::character varying");
            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.VehicleModelId).HasColumnName("VehicleModelID");

            entity.HasOne(d => d.Inventory).WithMany(p => p.SparePartTuHts)
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SparePart_TuHT_InventoryID_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.SparePartTuHts)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SparePart_TuHT_TypeID_fkey");
        });

        modelBuilder.Entity<SparePartTypeTuHt>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("SparePartType_TuHT_pkey");

            entity.ToTable("SparePartType_TuHT");

            entity.Property(e => e.TypeId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("TypeID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ACTIVE'::character varying");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<SparePartUsageHistoryTuHt>(entity =>
        {
            entity.HasKey(e => e.UsageId).HasName("SparePartUsageHistory_TuHT_pkey");

            entity.ToTable("SparePartUsageHistory_TuHT");

            entity.Property(e => e.UsageId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("UsageID");
            entity.Property(e => e.CenterId).HasColumnName("CenterID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SparePartId).HasColumnName("SparePartID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'ACTIVE'::character varying");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UsedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Center).WithMany(p => p.SparePartUsageHistoryTuHts)
                .HasForeignKey(d => d.CenterId)
                .HasConstraintName("SparePartUsageHistory_TuHT_CenterID_fkey");

            entity.HasOne(d => d.SparePart).WithMany(p => p.SparePartUsageHistoryTuHts)
                .HasForeignKey(d => d.SparePartId)
                .HasConstraintName("SparePartUsageHistory_TuHT_SparePartID_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserCuongtqld).HasName("User_pkey");

            entity.ToTable("User");

            entity.Property(e => e.UserCuongtqld).HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FullName).HasMaxLength(256);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("Vehicles_pkey");

            entity.Property(e => e.VehicleId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("VehicleID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Model).HasMaxLength(128);
            entity.Property(e => e.PlateNo).HasMaxLength(32);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");
            entity.Property(e => e.Vin)
                .HasMaxLength(64)
                .HasColumnName("VIN");

            entity.HasOne(d => d.Customer).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Vehicles_CustomerID_fkey");
        });

        modelBuilder.Entity<VehicleConditionDungVm>(entity =>
        {
            entity.HasKey(e => e.VehicleConditionDungVmid).HasName("VehicleConditionDungVM_pkey");

            entity.ToTable("VehicleConditionDungVM");

            entity.HasIndex(e => e.Status, "idx_vcdungvm_status");

            entity.HasIndex(e => e.VehicleId, "idx_vcdungvm_vehicle");

            entity.Property(e => e.VehicleConditionDungVmid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("VehicleConditionDungVMId");
            entity.Property(e => e.BodyStatus).HasMaxLength(200);
            entity.Property(e => e.BrakeStatus).HasMaxLength(200);
            entity.Property(e => e.Condition).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastMaintenance).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Status).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.VehicleConditionDungVms)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("VehicleConditionDungVM_VehicleId_fkey");
        });

        modelBuilder.Entity<WorkOrderApprovalThaoNtt>(entity =>
        {
            entity.HasKey(e => e.WoathaoNttid).HasName("WorkOrderApprovalThaoNTT_pkey");

            entity.ToTable("WorkOrderApprovalThaoNTT");

            entity.HasIndex(e => e.Method, "idx_thaontt_woa_method");

            entity.HasIndex(e => e.OrderId, "idx_thaontt_woa_order");

            entity.HasIndex(e => e.Status, "idx_thaontt_woa_status");

            entity.HasIndex(e => e.OrderId, "idx_woa_order");

            entity.HasIndex(e => e.OrderId, "uq_woa_order").IsUnique();

            entity.Property(e => e.WoathaoNttid)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("WOAThaoNTTId");
            entity.Property(e => e.ApprovedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Method).HasMaxLength(20);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Status).HasDefaultValue((short)0);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Order).WithOne(p => p.WorkOrderApprovalThaoNtt)
                .HasForeignKey<WorkOrderApprovalThaoNtt>(d => d.OrderId)
                .HasConstraintName("WorkOrderApprovalThaoNTT_OrderId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
