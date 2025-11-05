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
DROP TABLE IF EXISTS PaymentMethodCuongtq CASCADE;
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
    Code            VARCHAR(50),
    Name            VARCHAR(200) NOT NULL,
    Type            SMALLINT NOT NULL CHECK (Type IN (1,2,3)),
    Unit            VARCHAR(50),
    Status          VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Unique index on lower(code) when not null (case-insensitive)
CREATE UNIQUE INDEX IF NOT EXISTS ux_checklistitemthaontt_code_notnull ON checklistitemthaontt (lower(code)) WHERE code IS NOT NULL;

-- Additional indexes for filtering
CREATE INDEX IF NOT EXISTS ix_checklistitemthaontt_isactive ON checklistitemthaontt (isactive);
CREATE INDEX IF NOT EXISTS ix_checklistitemthaontt_status ON checklistitemthaontt (status);

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
    DayOfWeek       VARCHAR(3) NOT NULL,
    StartUtc        VARCHAR(5) NOT NULL,
    EndUtc          VARCHAR(5) NOT NULL,
    Slot            INT,
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
    BookingDate     DATE,
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
    Note            TEXT,
    Status          VARCHAR(50) DEFAULT 'PENDING' CHECK (Status IN ('PENDING','ASSIGNED','ACTIVE','DONE')),
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 23/34
CREATE TABLE ServiceIntakeThaoNTT (
    IntakeID        UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    BookingID       UUID NOT NULL UNIQUE REFERENCES BookingHuykt(BookingID) ON DELETE CASCADE,
    AdvisorID       UUID NOT NULL REFERENCES UserAccount(UserID) ON DELETE RESTRICT, -- Checked-in-by actor
    CheckinTimeUtc  TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    OdometerKm      INT,
    BatterySoC      INT,
    Notes           TEXT,
    IntakeResponseNote TEXT,
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

-- Ensure uniqueness of (IntakeID, ItemID) pairs for checklist responses
CREATE UNIQUE INDEX IF NOT EXISTS ux_checklistresponsethaontt_intake_item
    ON ChecklistResponseThaoNTT (IntakeID, ItemID);

-- BẢNG 25/34
CREATE TABLE OrderThaoNTT (
    OrderID         UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    CustomerID      UUID NOT NULL REFERENCES UserAccount(UserID) ON DELETE RESTRICT,
    VehicleID       UUID NOT NULL REFERENCES Vehicle(VehicleID) ON DELETE RESTRICT,
    BookingID       UUID UNIQUE REFERENCES BookingHuykt(BookingID) ON DELETE SET NULL,
    TotalAmount     DECIMAL(18,2) DEFAULT 0.00,
    Status          VARCHAR(50) DEFAULT 'PENDING_APPROVAL',
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BẢNG 26/34
CREATE TABLE WorkOrderApprovalThaoNTT (
    WOA_Id         UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    OrderID        UUID NOT NULL REFERENCES OrderThaoNTT(OrderID) ON DELETE CASCADE,
    Status         VARCHAR(50) DEFAULT 'AWAITING',
    ApprovedBy     UUID REFERENCES UserAccount(UserID),
    ApprovedAt     TIMESTAMP,
    Method         VARCHAR(20),
    Note           VARCHAR(500),
    IsActive       BOOLEAN DEFAULT TRUE,
    createdAt      TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt      TIMESTAMP DEFAULT CURRENT_TIMESTAMP
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

-- ENUM types
CREATE TYPE transaction_status AS ENUM ('CREATED', 'PAID', 'CANCELLED');
CREATE TYPE item_type AS ENUM ('SERVICE', 'SPAREPART');

-- Payment methods
CREATE TABLE PaymentMethodCuongtq (
    PaymentMethodID SERIAL PRIMARY KEY,
    Name            VARCHAR(256) NOT NULL,
    Description     VARCHAR(500),
    IsActive        BOOLEAN DEFAULT TRUE,
    createdAt       TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updatedAt       TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- Transactions
CREATE TABLE TransactionCuongtq (
    TransactionID   UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    OrderID         UUID NOT NULL UNIQUE
        REFERENCES OrderThaoNTT(OrderID) ON DELETE CASCADE,
    PaymentMethodID UUID
        REFERENCES PaymentMethodCuongtq(PaymentMethodID),
    Description     VARCHAR(500),
    Status          transaction_status NOT NULL DEFAULT 'CREATED',
    Reason          VARCHAR(50),
    TotalAmount     DECIMAL(18,2) NOT NULL,
    createdAt       TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updatedAt       TIMESTAMP WITH TIME ZONE DEFAULT now(),
    StaffID         UUID NOT NULL
        REFERENCES UserAccount(UserID)
);

-- Receipts
CREATE TABLE ReceiptCuongtq (
    ReceiptID       UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    TotalAmount     DECIMAL(18,2) NOT NULL,
    CustomerID      UUID NOT NULL
        REFERENCES UserAccount(UserID),
    TransactionID   UUID
        REFERENCES TransactionCuongtq(TransactionID),
    createdAt       TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updatedAt       TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- Receipt items
CREATE TABLE ReceiptItemCuongtq (
    ReceiptItemID   UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    ReceiptID       UUID NOT NULL
        REFERENCES ReceiptCuongtq(ReceiptID) ON DELETE CASCADE,
    ItemType        item_type NOT NULL,
    ItemName        VARCHAR(256) NOT NULL,
    ItemID          UUID,
    Quantity        INT NOT NULL CHECK (Quantity > 0),
    UnitPrice       DECIMAL(18,2) NOT NULL CHECK (UnitPrice >= 0),
    LineTotal       DECIMAL(18,2) NOT NULL CHECK (LineTotal >= 0),
    createdAt       TIMESTAMP WITH TIME ZONE DEFAULT now(),
    updatedAt       TIMESTAMP WITH TIME ZONE DEFAULT now()
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
CREATE TRIGGER trg_update_paymentcuongtq_updatedAt BEFORE UPDATE ON PaymentMethodCuongtq FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
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
CREATE TRIGGER trg_update_receiptitemcuongtq_updatedAt BEFORE UPDATE ON TransactionCuongtq FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_usage_updatedAt BEFORE UPDATE ON SparePartUsageHistory_TuHT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_forecast_updatedAt BEFORE UPDATE ON SparePartForecast_TuHT FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
CREATE TRIGGER trg_update_replenishment_updatedAt BEFORE UPDATE ON SparePartReplenishmentRequest FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
-- Lưu ý: Không tạo trigger cho bảng RefreshToken
INSERT INTO "userrole"
(roleid, "name", description)
VALUES('af4578f0-a8be-429c-8c3f-c9be22c9368b'::uuid, 'ADMIN', 'Quản trị viên hệ thống — có toàn quyền quản lý người dùng, trung tâm, kho và dữ liệu hệ thống.');
INSERT INTO "userrole"
(roleid, "name", description)
VALUES('8bce91c5-0f07-4146-91e0-938b2dbc2104'::uuid, 'STAFF', 'Nhân viên điều phối hoặc hỗ trợ quản lý nghiệp vụ, có quyền xem và cập nhật dữ liệu nghiệp vụ trong phạm vi cho phép.');
INSERT INTO "userrole"
(roleid, "name", description)
VALUES('1d32603c-f69c-452e-b655-740f7ec48586'::uuid, 'TECHNICIAN', 'Kỹ thuật viên — chịu trách nhiệm bảo trì, thay thế và cập nhật lịch sử sử dụng phụ tùng tại trung tâm.');
INSERT INTO "userrole"
(roleid, "name", description)
VALUES('55ca452e-fe3e-4e7c-b594-65263c738f0d'::uuid, 'CUSTOMER', 'Khách hàng hoặc người dùng cuối — có thể gửi yêu cầu, xem trạng thái dịch vụ và lịch sử bảo trì.');

INSERT INTO useraccount
(userid, email, "password", lastname, firstname, address, phonenumber, roleid, status, isactive, createdat, updatedat)
VALUES('431eaeb4-3c02-483c-ba01-7398780333c9'::uuid, 'admin@example.com', 'AQAAAAIAAYagAAAAEKyeet50VJU7arWO7JxiqBpx7NhxnFoacKjfNLF8xLUMAV5N+U412w2ylHjKlYOrQg==', 'string', 'string', 'string', '0000000000', 'af4578f0-a8be-429c-8c3f-c9be22c9368b'::uuid, 'ACTIVE', true, '2025-10-15 05:44:24.987', '2025-10-15 05:44:24.987');
