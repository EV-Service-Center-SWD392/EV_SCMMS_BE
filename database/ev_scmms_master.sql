-- =========================================================
-- PHẦN 1: CÀI ĐẶT BAN ĐẦU
-- =========================================================

-- Kích hoạt extension để sử dụng UUID
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Hàm trigger để tự động cập nhật cột 'updatedAt'
CREATE OR REPLACE FUNCTION update_updatedAt()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updatedAt = CURRENT_TIMESTAMP;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- =========================================================
-- 2) Minimal base tables for FK (User, Vehicles, Centers)
-- =========================================================
DROP TABLE IF EXISTS "Vehicles" CASCADE;
DROP TABLE IF EXISTS "User" CASCADE;
DROP TABLE IF EXISTS "Center" CASCADE;

CREATE TABLE "Center" (
  "CenterID"  UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  "Name"      VARCHAR(256) NOT NULL,
  "Address"   VARCHAR(256),
  "Status"    VARCHAR(50) DEFAULT 'ACTIVE',
  "IsActive"  BOOLEAN DEFAULT TRUE,
  "createdAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "updatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "User" (
    "UserCuongtqld" UUID PRIMARY KEY DEFAULT uuid_generate_v4(), 
    "UserName"      VARCHAR(256),                                 
    "Email"         VARCHAR(256),                                 
    "PasswordHash"  TEXT,                                         
    "PhoneNumber"   VARCHAR(50),                                  
    "Address"       VARCHAR(500),                                
    "CreatedAt"     TIMESTAMP DEFAULT CURRENT_TIMESTAMP,           
    "UpdatedAt"     TIMESTAMP DEFAULT CURRENT_TIMESTAMP,           
    "FullName"      VARCHAR(256)                                 
);

CREATE TABLE "Vehicles" (
  "VehicleID" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  "CustomerID" UUID REFERENCES "User"("UserCuongtqld") ON DELETE SET NULL,
  "VIN"       VARCHAR(64),
  "PlateNo"   VARCHAR(32),
  "Model"     VARCHAR(128),
  "createdAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "updatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TRIGGER trg_center_upd BEFORE UPDATE ON "Center"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_user_upd BEFORE UPDATE ON "User"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_vehicles_upd BEFORE UPDATE ON "Vehicles"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

-- =========================================================
-- 3) B1/B2/B3/B4 core tables (EV service flow)
-- =========================================================
-- 3.1 Booking schedule (slots for a center)
DROP TABLE IF EXISTS "BookingScheduleThaoNTT" CASCADE;
CREATE TABLE "BookingScheduleThaoNTT" (
  "BSThaoNTTId" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  "CenterId"    UUID NOT NULL REFERENCES "Center"("CenterID") ON DELETE CASCADE,
  "StartUtc"    TIMESTAMP NOT NULL,
  "EndUtc"      TIMESTAMP NOT NULL,
  "Capacity"    INT NOT NULL DEFAULT 1,
  "Status"      SMALLINT NOT NULL DEFAULT 0,  -- 0=Open,1=Closed
  "Note"        VARCHAR(500),
  "createdAt"   TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "updatedAt"   TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT uq_schedule_slot UNIQUE ("CenterId","StartUtc","EndUtc")
);
CREATE INDEX idx_sched_center_start ON "BookingScheduleThaoNTT"("CenterId","StartUtc");

CREATE TRIGGER trg_sched_upd BEFORE UPDATE ON "BookingScheduleThaoNTT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

-- 3.2 Booking (customer booking tied to a vehicle & schedule)
DROP TABLE IF EXISTS "BookingThaoNTT" CASCADE;
CREATE TABLE "BookingThaoNTT" (
  "BookingThaoNTTId" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  "CustomerId" UUID NOT NULL REFERENCES "User"("UserCuongtqld") ON DELETE RESTRICT,
  "VehicleId"  UUID NOT NULL REFERENCES "Vehicles"("VehicleID") ON DELETE RESTRICT,
  "BookingScheduleId" UUID REFERENCES "BookingScheduleThaoNTT"("BSThaoNTTId") ON DELETE SET NULL,
  "Status"    SMALLINT NOT NULL DEFAULT 0, -- 0=Pending,1=Confirmed,2=Canceled,3=NoShow,4=CheckedIn,5=Converted
  "Notes"     VARCHAR(1000),
  "createdAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "updatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_booking_customer ON "BookingThaoNTT"("CustomerId");
CREATE INDEX idx_booking_vehicle ON "BookingThaoNTT"("VehicleId");

CREATE TRIGGER trg_booking_upd BEFORE UPDATE ON "BookingThaoNTT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

-- 3.3 Service Intake (checked-in info)
DROP TABLE IF EXISTS "ServiceIntakeThaoNTT" CASCADE;
CREATE TABLE "ServiceIntakeThaoNTT" (
  "SIThaoNTTId" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  "BookingId"   UUID NOT NULL REFERENCES "BookingThaoNTT"("BookingThaoNTTId") ON DELETE CASCADE,
  "AdvisorId"   UUID REFERENCES "User"("UserCuongtqld") ON DELETE SET NULL,
  "CheckinTimeUtc" TIMESTAMP,
  "OdometerKm"  INT,
  "BatterySoC"  INT,
  "Notes"       VARCHAR(1000),
  "createdAt"   TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "updatedAt"   TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_intake_booking ON "ServiceIntakeThaoNTT"("BookingId");

CREATE TRIGGER trg_intake_upd BEFORE UPDATE ON "ServiceIntakeThaoNTT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

-- 3.4 Checklist master & responses
DROP TABLE IF EXISTS "ChecklistItemThaoNTT" CASCADE;
CREATE TABLE "ChecklistItemThaoNTT" (
  "CLIThaoNTTId" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  "Code"   VARCHAR(50) UNIQUE,
  "Name"   VARCHAR(100) NOT NULL,
  "Type"   SMALLINT NOT NULL DEFAULT 0,   -- 0=Bool,1=Number,2=Text
  "Unit"   VARCHAR(20),
  "IsActive" BOOLEAN DEFAULT TRUE,
  "createdAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "updatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE TRIGGER trg_chkitem_upd BEFORE UPDATE ON "ChecklistItemThaoNTT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

DROP TABLE IF EXISTS "ChecklistResponseThaoNTT" CASCADE;
CREATE TABLE "ChecklistResponseThaoNTT" (
  "CRThaoNTTId" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  "IntakeId"  UUID NOT NULL REFERENCES "ServiceIntakeThaoNTT"("SIThaoNTTId") ON DELETE CASCADE,
  "ItemId"    UUID NOT NULL REFERENCES "ChecklistItemThaoNTT"("CLIThaoNTTId") ON DELETE RESTRICT,
  "ValueBool"   BOOLEAN,
  "ValueNumber" DECIMAL(10,2),
  "ValueText"   VARCHAR(500),
  "Severity"    SMALLINT,
  "Comment"     VARCHAR(500),
  "PhotoUrl"    VARCHAR(300),
  "createdAt"   TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "updatedAt"   TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_resp_intake ON "ChecklistResponseThaoNTT"("IntakeId");
CREATE INDEX idx_resp_item   ON "ChecklistResponseThaoNTT"("ItemId");

CREATE TRIGGER trg_chkresp_upd BEFORE UPDATE ON "ChecklistResponseThaoNTT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

-- 3.5 Assignment (technician assignment)
DROP TABLE IF EXISTS "AssignmentThaoNTT" CASCADE;
CREATE TABLE "AssignmentThaoNTT" (
  "AssThaoNTTId" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  "BookingId"    UUID NOT NULL REFERENCES "BookingThaoNTT"("BookingThaoNTTId") ON DELETE CASCADE,
  "TechnicianId" UUID NOT NULL REFERENCES "User"("UserCuongtqld") ON DELETE RESTRICT,
  "PlannedStartUtc" TIMESTAMP,
  "PlannedEndUtc"   TIMESTAMP,
  "Status"       VARCHAR(50),
  "QueueNo"      INT,
  "createdAt"    TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "updatedAt"    TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_assign_booking ON "AssignmentThaoNTT"("BookingId");
CREATE INDEX idx_assign_tech    ON "AssignmentThaoNTT"("TechnicianId");

CREATE TRIGGER trg_assign_upd BEFORE UPDATE ON "AssignmentThaoNTT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

-- 3.6 Services catalog (optional but useful for Order lines)
DROP TABLE IF EXISTS "Service" CASCADE;
CREATE TABLE "Service" (
  "Id" SERIAL PRIMARY KEY,
  "Name"        VARCHAR(256) NOT NULL,
  "Description" VARCHAR(2000),
  "DurationMinutes" INT,
  "BasePrice"   DECIMAL(18,2),
  "createdAt"   TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "updatedAt"   TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE TRIGGER trg_service_upd BEFORE UPDATE ON "Service"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

-- 3.7 Work Order (Orders) + lines + approval
DROP TABLE IF EXISTS "OrderServiceThaoNTT" CASCADE;
DROP TABLE IF EXISTS "OrderThaoNTT" CASCADE;
DROP TABLE IF EXISTS "WorkOrderApprovalThaoNTT" CASCADE;

CREATE TABLE "OrderThaoNTT" (
  "OrderThaoNTTId" SERIAL PRIMARY KEY,
  "CustomerId" UUID REFERENCES "User"("UserCuongtqld") ON DELETE SET NULL,
  "VehicleId"  UUID REFERENCES "Vehicles"("VehicleID") ON DELETE SET NULL,
  "BookingId"  UUID REFERENCES "BookingThaoNTT"("BookingThaoNTTId") ON DELETE SET NULL,
  "Status"     SMALLINT NOT NULL DEFAULT 0,
  "TotalAmount" DECIMAL(18,2) DEFAULT 0,
  "createdAt"  TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "updatedAt"  TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "PaymentId"  INT
);
CREATE INDEX idx_order_customer ON "OrderThaoNTT"("CustomerId");
CREATE INDEX idx_order_booking  ON "OrderThaoNTT"("BookingId");

CREATE TRIGGER trg_order_upd BEFORE UPDATE ON "OrderThaoNTT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

CREATE TABLE "OrderServiceThaoNTT" (
  "OSThaoNTTId" SERIAL PRIMARY KEY,
  "OrderId" INT NOT NULL REFERENCES "OrderThaoNTT"("OrderThaoNTTId") ON DELETE CASCADE,
  "ServiceId" INT REFERENCES "Service"("Id") ON DELETE SET NULL,
  "Quantity"  INT NOT NULL DEFAULT 1,
  "UnitPrice" DECIMAL(18,2) NOT NULL DEFAULT 0,
  "createdAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "updatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_os_order ON "OrderServiceThaoNTT"("OrderId");

CREATE TRIGGER trg_os_upd BEFORE UPDATE ON "OrderServiceThaoNTT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

CREATE TABLE "WorkOrderApprovalThaoNTT" (
  "WOAThaoNTTId" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  "OrderId"   INT NOT NULL REFERENCES "OrderThaoNTT"("OrderThaoNTTId") ON DELETE CASCADE,
  "Status"    SMALLINT NOT NULL DEFAULT 0,
  "ApprovedAt" TIMESTAMP,
  "Method"    VARCHAR(20),
  "Note"      VARCHAR(500),
  "createdAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "updatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_woa_order ON "WorkOrderApprovalThaoNTT"("OrderId");

-- =========================================================
-- PHẦN 2: XÓA TOÀN BỘ CÁC BẢNG CŨ ĐỂ LÀM SẠCH
-- =========================================================
DROP TABLE IF EXISTS RefreshToken CASCADE;
DROP TABLE IF EXISTS SparePartReplenishmentRequest CASCADE;
DROP TABLE IF EXISTS SparePartForecast_TuHT CASCADE;
DROP TABLE IF EXISTS SparePartUsageHistory_TuHT CASCADE;
DROP TABLE IF EXISTS MaintenanceTaskDungVM CASCADE;
DROP TABLE IF EXISTS MaintenanceHistoryDungVM CASCADE;
DROP TABLE IF EXISTS ReceiptItemCuongtq CASCADE;
DROP TABLE IF EXISTS ReceiptCuongtq CASCADE;
DROP TABLE IF EXISTS OrderSparePart CASCADE;
DROP TABLE IF EXISTS OrderServiceThaoNTT CASCADE;
DROP TABLE IF EXISTS WorkOrderApprovalThaoNTT CASCADE;
DROP TABLE IF EXISTS OrderThaoNTT CASCADE;
DROP TABLE IF EXISTS PaymentCuongtq CASCADE;
DROP TABLE IF EXISTS Service CASCADE;
DROP TABLE IF EXISTS ChecklistResponseThaoNTT CASCADE;
DROP TABLE IF EXISTS ChecklistItemThaoNTT CASCADE;
DROP TABLE IF EXISTS ServiceIntakeThaoNTT CASCADE;
DROP TABLE IF EXISTS AssignmentThaoNTT CASCADE;
DROP TABLE IF EXISTS BookingStatusLogHuykt CASCADE;
DROP TABLE IF EXISTS BookingHuykt CASCADE;
DROP TABLE IF EXISTS BookingSchedule CASCADE;
DROP TABLE IF EXISTS UserWorkScheduleTuantm CASCADE;
DROP TABLE IF EXISTS WorkScheduleTuantm CASCADE;
DROP TABLE IF EXISTS UserCenterTuantm CASCADE;
DROP TABLE IF EXISTS SparePart_TuHT CASCADE;
DROP TABLE IF EXISTS SparePartType_TuHT CASCADE;
DROP TABLE IF EXISTS Inventory_TuHT CASCADE;
DROP TABLE IF EXISTS VehicleConditionDungVM CASCADE;
DROP TABLE IF EXISTS Vehicle CASCADE;
DROP TABLE IF EXISTS VehicleModel CASCADE;
DROP TABLE IF EXISTS UserCertificateTuantm CASCADE;
DROP TABLE IF EXISTS CertificateTuantm CASCADE;
DROP TABLE IF EXISTS UserAccount CASCADE;
DROP TABLE IF EXISTS UserRole CASCADE;
DROP TABLE IF EXISTS CenterTuantm CASCADE;


-- =========================================================
-- PHẦN 3: TẠO CẤU TRÚC CÁC BẢNG
-- =========================================================

-- Bảng dữ liệu gốc (Master Data)
-- ---------------------------------------------------------

-- BẢNG 1/34
CREATE TABLE CenterTuantm (
    CenterID        UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name            VARCHAR(256) NOT NULL,
    Address         VARCHAR(256),
    Status          VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 2/34
CREATE TABLE UserRole (
    RoleID          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name            VARCHAR(128) NOT NULL UNIQUE,
    Description     VARCHAR(500),
    Status          VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 3/34
CREATE TABLE VehicleModel (
    ModelID         SERIAL PRIMARY KEY,
    Name            VARCHAR(256) NOT NULL,
    Brand           VARCHAR(128),
    EngineType      VARCHAR(128),
    Status          VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 4/34
CREATE TABLE Service (
    ServiceID       UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name            VARCHAR(256) NOT NULL,
    Description     TEXT,
    DurationMinutes INT,
    BasePrice       DECIMAL(18,2) DEFAULT 0.00,
    Status          VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 5/34
CREATE TABLE PaymentCuongtq (
    PaymentID       SERIAL PRIMARY KEY,
    Name            VARCHAR(256) NOT NULL,
    Description     VARCHAR(500),
    Status          VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 6/34
CREATE TABLE CertificateTuantm (
    CertificateID   UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name            VARCHAR(256) NOT NULL,
    Description     TEXT,
    Status          VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 7/34
CREATE TABLE ChecklistItemThaoNTT (
    ItemID          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Code            VARCHAR(50) UNIQUE,
    Name            VARCHAR(100) NOT NULL,
    Type            SMALLINT,
    Unit            VARCHAR(20),
    Status          VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 8/34
CREATE TABLE SparePartType_TuHT (
    TypeID          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name            VARCHAR(256) NOT NULL,
    Description     VARCHAR(500),
    Status          VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng phụ thuộc & nghiệp vụ
-- ---------------------------------------------------------

-- BẢNG 9/34
CREATE TABLE UserAccount (
    UserID          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    RoleID          UUID NOT NULL REFERENCES UserRole(RoleID),
    Email           VARCHAR(256) NOT NULL UNIQUE,
    Password        VARCHAR(256) NOT NULL,
    PhoneNumber     VARCHAR(15),
    Address         VARCHAR(256),
    LastName        VARCHAR(100),
    FirstName       VARCHAR(100),
    Status          VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 10/34
CREATE TABLE RefreshToken (
    TokenID         UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    UserID          UUID NOT NULL REFERENCES UserAccount(UserID) ON DELETE CASCADE,
    Token           VARCHAR(512) NOT NULL UNIQUE,
    ExpiresAt       TIMESTAMP NOT NULL,
    Status          VARCHAR(50) DEFAULT 'VALID',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 11/34
CREATE TABLE Inventory_TuHT (
    InventoryID         UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    CenterID            UUID NOT NULL REFERENCES CenterTuantm(CenterID) ON DELETE CASCADE,
    Quantity            INT DEFAULT 0,
    MinimumStockLevel   INT DEFAULT 0,
    Status              VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive            BOOLEAN DEFAULT TRUE,
    createdAt           TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt           TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 12/34
CREATE TABLE WorkScheduleTuantm (
    WorkScheduleID  UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    CenterID        UUID NOT NULL REFERENCES CenterTuantm(CenterID) ON DELETE CASCADE,
    StartTime       TIMESTAMP NOT NULL,
    EndTime         TIMESTAMP NOT NULL,
    Status          VARCHAR(50) DEFAULT 'PLANNED',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 13/34
CREATE TABLE BookingSchedule (
    SlotID          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    CenterID        UUID NOT NULL REFERENCES CenterTuantm(CenterID) ON DELETE CASCADE,
    StartUtc        TIMESTAMP NOT NULL,
    EndUtc          TIMESTAMP NOT NULL,
    Capacity        INT DEFAULT 1,
    Note            VARCHAR(500),
    Status          VARCHAR(50) DEFAULT 'OPEN',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 14/34
CREATE TABLE Vehicle (
    VehicleID       UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    CustomerID      UUID NOT NULL REFERENCES UserAccount(UserID) ON DELETE CASCADE,
    ModelID         INT REFERENCES VehicleModel(ModelID) ON DELETE SET NULL,
    LicensePlate    VARCHAR(50) NOT NULL UNIQUE,
    Year            INT,
    Color           VARCHAR(64),
    Status          VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 15/34
CREATE TABLE UserCenterTuantm (
    UserCenterID    UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    UserID          UUID NOT NULL REFERENCES UserAccount(UserID) ON DELETE CASCADE,
    CenterID        UUID NOT NULL REFERENCES CenterTuantm(CenterID) ON DELETE CASCADE,
    WorkSince       TIMESTAMP,
    Status          VARCHAR(50) DEFAULT 'WORKING',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(UserID, CenterID)
);

-- BẢNG 16/34
CREATE TABLE UserCertificateTuantm (
    UserCertificateID UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    UserID          UUID NOT NULL REFERENCES UserAccount(UserID) ON DELETE CASCADE,
    CertificateID   UUID NOT NULL REFERENCES CertificateTuantm(CertificateID) ON DELETE CASCADE,
    Status          VARCHAR(50) DEFAULT 'VALID',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(UserID, CertificateID)
);

-- BẢNG 17/34
CREATE TABLE UserWorkScheduleTuantm (
    UserWorkScheduleID UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    UserID          UUID NOT NULL REFERENCES UserAccount(UserID) ON DELETE CASCADE,
    WorkScheduleID  UUID NOT NULL REFERENCES WorkScheduleTuantm(WorkScheduleID) ON DELETE CASCADE,
    Status          VARCHAR(50) DEFAULT 'ASSIGNED',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(UserID, WorkScheduleID)
);

-- BẢNG 18/34
CREATE TABLE SparePart_TuHT (
    SparePartID     UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    VehicleModelID  INT REFERENCES VehicleModel(ModelID) ON DELETE SET NULL,
    InventoryID     UUID REFERENCES Inventory_TuHT(InventoryID) ON DELETE SET NULL,
    TypeID          UUID REFERENCES SparePartType_TuHT(TypeID) ON DELETE SET NULL,
    Name            VARCHAR(256) NOT NULL,
    UnitPrice       DECIMAL(18,2),
    Manufacture     VARCHAR(256),
    Status          VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 19/34
CREATE TABLE VehicleConditionDungVM (
    ConditionID     UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    VehicleID       UUID NOT NULL REFERENCES Vehicle(VehicleID) ON DELETE CASCADE,
    LastMaintenance TIMESTAMP,
    Condition       TEXT,
    BatteryHealth   INT,
    TirePressure    INT,
    BrakeStatus     VARCHAR(256),
    BodyStatus      VARCHAR(256),
    Status          VARCHAR(50) DEFAULT 'RECORDED',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 20/34
CREATE TABLE BookingHuykt (
    BookingID       UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    CustomerID      UUID NOT NULL REFERENCES UserAccount(UserID) ON DELETE CASCADE,
    VehicleID       UUID NOT NULL REFERENCES Vehicle(VehicleID) ON DELETE CASCADE,
    SlotID          UUID REFERENCES BookingSchedule(SlotID) ON DELETE SET NULL,
    Notes           TEXT,
    Status          VARCHAR(50) DEFAULT 'PENDING',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 21/34
CREATE TABLE BookingStatusLogHuykt (
    LogID           UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    BookingID       UUID NOT NULL REFERENCES BookingHuykt(BookingID) ON DELETE CASCADE,
    Status          VARCHAR(50) NOT NULL,
    IsSeen          BOOLEAN DEFAULT FALSE,
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 22/34
CREATE TABLE AssignmentThaoNTT (
    AssignmentID    UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    BookingID       UUID NOT NULL REFERENCES BookingHuykt(BookingID) ON DELETE CASCADE,
    TechnicianID    UUID NOT NULL REFERENCES UserAccount(UserID) ON DELETE RESTRICT,
    PlannedStartUtc TIMESTAMP,
    PlannedEndUtc   TIMESTAMP,
    QueueNo         INT,
    Status          VARCHAR(50) DEFAULT 'PENDING',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 23/34
CREATE TABLE ServiceIntakeThaoNTT (
    IntakeID        UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    BookingID       UUID NOT NULL UNIQUE REFERENCES BookingHuykt(BookingID) ON DELETE CASCADE,
    AdvisorID       UUID NOT NULL REFERENCES UserAccount(UserID) ON DELETE RESTRICT,
    CheckinTimeUtc  TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    OdometerKm      INT,
    BatterySoC      INT,
    Notes           TEXT,
    Status          VARCHAR(50) DEFAULT 'CHECKED_IN',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 24/34
CREATE TABLE ChecklistResponseThaoNTT (
    ResponseID      UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    IntakeID        UUID NOT NULL REFERENCES ServiceIntakeThaoNTT(IntakeID) ON DELETE CASCADE,
    ItemID          UUID NOT NULL REFERENCES ChecklistItemThaoNTT(ItemID) ON DELETE CASCADE,
    ValueBool       BOOLEAN,
    ValueNumber     DECIMAL(10,2),
    ValueText       VARCHAR(500),
    Severity        SMALLINT,
    Comment         VARCHAR(500),
    PhotoUrl        VARCHAR(300),
    Status          VARCHAR(50) DEFAULT 'COMPLETED',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 25/34
CREATE TABLE OrderThaoNTT (
    OrderID         UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    CustomerID      UUID NOT NULL REFERENCES UserAccount(UserID) ON DELETE RESTRICT,
    VehicleID       UUID NOT NULL REFERENCES Vehicle(VehicleID) ON DELETE RESTRICT,
    BookingID       UUID UNIQUE REFERENCES BookingHuykt(BookingID) ON DELETE SET NULL,
    PaymentID       INT REFERENCES PaymentCuongtq(PaymentID) ON DELETE SET NULL,
    TotalAmount     DECIMAL(18,2) DEFAULT 0.00,
    Status          VARCHAR(50) DEFAULT 'PENDING_APPROVAL',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 26/34
CREATE TABLE WorkOrderApprovalThaoNTT (
    WOA_ID          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    OrderID         UUID NOT NULL REFERENCES OrderThaoNTT(OrderID) ON DELETE CASCADE,
    Status          VARCHAR(50) DEFAULT 'AWAITING', -- AWAITING, APPROVED, REJECTED
    ApprovedAt      TIMESTAMP,
    Method          VARCHAR(20), -- App, InPerson, ESign
    Note            VARCHAR(500),
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 27/34
CREATE TABLE OrderServiceThaoNTT (
    OrderDetailID   UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    OrderID         UUID NOT NULL REFERENCES OrderThaoNTT(OrderID) ON DELETE CASCADE,
    ServiceID       UUID NOT NULL REFERENCES Service(ServiceID) ON DELETE RESTRICT,
    Quantity        INT DEFAULT 1,
    UnitPrice       DECIMAL(18,2),
    Status          VARCHAR(50) DEFAULT 'ADDED',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 28/34
CREATE TABLE OrderSparePart (
    OrderSparePartID UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    OrderID         UUID NOT NULL REFERENCES OrderThaoNTT(OrderID) ON DELETE CASCADE,
    SparePartID     UUID NOT NULL REFERENCES SparePart_TuHT(SparePartID) ON DELETE RESTRICT,
    Quantity        INT NOT NULL,
    UnitPrice       DECIMAL(18,2),
    Discount        DECIMAL(18,2) DEFAULT 0.00,
    Status          VARCHAR(50) DEFAULT 'ADDED',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 29/34
CREATE TABLE MaintenanceHistoryDungVM (
    HistoryID       UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    VehicleID       UUID NOT NULL REFERENCES Vehicle(VehicleID) ON DELETE CASCADE,
    OrderID         UUID REFERENCES OrderThaoNTT(OrderID) ON DELETE SET NULL,
    CompletedDate   TIMESTAMP NOT NULL,
    Summary         TEXT,
    Status          VARCHAR(50) DEFAULT 'LOGGED',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 30/34
CREATE TABLE MaintenanceTaskDungVM (
    TaskID          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    OrderDetailID   UUID NOT NULL REFERENCES OrderServiceThaoNTT(OrderDetailID) ON DELETE CASCADE,
    TechnicianID    UUID REFERENCES UserAccount(UserID) ON DELETE SET NULL,
    Description     TEXT,
    Task            VARCHAR(256),
    Status          VARCHAR(50) DEFAULT 'PENDING',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 31/34
CREATE TABLE ReceiptCuongtq (
    ReceiptID       UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    OrderID         UUID NOT NULL UNIQUE REFERENCES OrderThaoNTT(OrderID) ON DELETE CASCADE,
    PaymentMethod   VARCHAR(128),
    TotalAmount     DECIMAL(18,2) NOT NULL,
    CustomerID      UUID NOT NULL REFERENCES UserAccount(UserID),
    StaffID         UUID NOT NULL REFERENCES UserAccount(UserID),
    Status          VARCHAR(50) DEFAULT 'PAID',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 32/34
CREATE TABLE ReceiptItemCuongtq (
    ReceiptItemID   UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    ReceiptID       UUID NOT NULL REFERENCES ReceiptCuongtq(ReceiptID) ON DELETE CASCADE,
    ItemType        VARCHAR(50) NOT NULL,
    ItemName        VARCHAR(256) NOT NULL,
    ItemCode        VARCHAR(128),
    Quantity        INT NOT NULL,
    UnitPrice       DECIMAL(18,2) NOT NULL,
    LineTotal       DECIMAL(18,2) NOT NULL,
    Status          VARCHAR(50) DEFAULT 'INCLUDED',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 33/34
CREATE TABLE SparePartUsageHistory_TuHT (
    UsageID         UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    SparePartID     UUID NOT NULL REFERENCES SparePart_TuHT(SparePartID) ON DELETE CASCADE,
    CenterID        UUID NOT NULL REFERENCES CenterTuantm(CenterID) ON DELETE CASCADE,
    OrderSparePartID UUID REFERENCES OrderSparePart(OrderSparePartID) ON DELETE SET NULL,
    QuantityUsed    INT NOT NULL,
    UsedDate        TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Status          VARCHAR(50) DEFAULT 'USED',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 34/34
CREATE TABLE SparePartForecast_TuHT (
    ForecastID          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    SparePartID         UUID NOT NULL REFERENCES SparePart_TuHT(SparePartID) ON DELETE CASCADE,
    CenterID            UUID NOT NULL REFERENCES CenterTuantm(CenterID) ON DELETE CASCADE,
    PredictedUsage      INT DEFAULT 0,
    SafetyStock         INT DEFAULT 0,
    ReorderPoint        INT DEFAULT 0,
    ForecastedBy        VARCHAR(64) DEFAULT 'AI',
    ForecastConfidence  DECIMAL(5,2),
    ForecastDate        TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ApprovedBy          UUID,
    ApprovedDate        TIMESTAMP,
    Status              VARCHAR(50) DEFAULT 'PENDING',
    IsActive            BOOLEAN DEFAULT TRUE,
    createdAt           TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt           TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 35/35 (Bảng này không có trong lần đếm trước)
CREATE TABLE SparePartReplenishmentRequest (
    RequestID       UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    CenterID        UUID NOT NULL REFERENCES CenterTuantm(CenterID) ON DELETE CASCADE,
    SparePartID     UUID NOT NULL REFERENCES SparePart_TuHT(SparePartID) ON DELETE CASCADE,
    ForecastID      UUID REFERENCES SparePartForecast_TuHT(ForecastID) ON DELETE SET NULL,
    SuggestedQuantity INT DEFAULT 0,
    ApprovedBy      UUID,
    ApprovedAt      TIMESTAMP,
    Notes           VARCHAR(500),
    Status          VARCHAR(50) DEFAULT 'PENDING',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


-- =========================================================
-- PHẦN 4: TẠO TRIGGER CẬP NHẬT TỰ ĐỘNG
-- =========================================================
CREATE TRIGGER trg_update_centertuantm_updatedAt BEFORE UPDATE ON CenterTuantm FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_userrole_updatedAt BEFORE UPDATE ON UserRole FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_vehiclemodel_updatedAt BEFORE UPDATE ON VehicleModel FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_service_updatedAt BEFORE UPDATE ON Service FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_paymentcuongtq_updatedAt BEFORE UPDATE ON PaymentCuongtq FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_certificatetuantm_updatedAt BEFORE UPDATE ON CertificateTuantm FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_checklistitemthaontt_updatedAt BEFORE UPDATE ON ChecklistItemThaoNTT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_spareparttype_updatedAt BEFORE UPDATE ON SparePartType_TuHT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_useraccount_updatedAt BEFORE UPDATE ON UserAccount FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_inventory_updatedAt BEFORE UPDATE ON Inventory_TuHT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_workscheduletuantm_updatedAt BEFORE UPDATE ON WorkScheduleTuantm FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_bookingschedule_updatedAt BEFORE UPDATE ON BookingSchedule FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_vehicle_updatedAt BEFORE UPDATE ON Vehicle FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_usercentertuantm_updatedAt BEFORE UPDATE ON UserCenterTuantm FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_usercertificatetuantm_updatedAt BEFORE UPDATE ON UserCertificateTuantm FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_userworkscheduletuantm_updatedAt BEFORE UPDATE ON UserWorkScheduleTuantm FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_sparepart_updatedAt BEFORE UPDATE ON SparePart_TuHT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_vehicleconditiondungvm_updatedAt BEFORE UPDATE ON VehicleConditionDungVM FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_bookinghuykt_updatedAt BEFORE UPDATE ON BookingHuykt FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_bookingstatusloghuykt_updatedAt BEFORE UPDATE ON BookingStatusLogHuykt FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_assignmentthaontt_updatedAt BEFORE UPDATE ON AssignmentThaoNTT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_serviceintakethaontt_updatedAt BEFORE UPDATE ON ServiceIntakeThaoNTT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_checklistresponsethaontt_updatedAt BEFORE UPDATE ON ChecklistResponseThaoNTT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_orderthaontt_updatedAt BEFORE UPDATE ON OrderThaoNTT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_workorderapprovalthaontt_updatedAt BEFORE UPDATE ON WorkOrderApprovalThaoNTT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_orderservicethaontt_updatedAt BEFORE UPDATE ON OrderServiceThaoNTT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_ordersparepart_updatedAt BEFORE UPDATE ON OrderSparePart FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_maintenancehistorydungvm_updatedAt BEFORE UPDATE ON MaintenanceHistoryDungVM FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_maintenancetaskdungvm_updatedAt BEFORE UPDATE ON MaintenanceTaskDungVM FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_receiptcuongtq_updatedAt BEFORE UPDATE ON ReceiptCuongtq FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_receiptitemcuongtq_updatedAt BEFORE UPDATE ON ReceiptItemCuongtq FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_usage_updatedAt BEFORE UPDATE ON SparePartUsageHistory_TuHT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_forecast_updatedAt BEFORE UPDATE ON SparePartForecast_TuHT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_replenishment_updatedAt BEFORE UPDATE ON SparePartReplenishmentRequest FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
-- Lưu ý: Không tạo trigger cho bảng RefreshToken
