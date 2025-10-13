-- In psql, switch to the new DB:
-- \c postgress

-- =========================================================
-- 1) Extensions & helper trigger
-- =========================================================
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

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

CREATE TRIGGER trg_woa_upd BEFORE UPDATE ON "WorkOrderApprovalThaoNTT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

-- =========================================================
-- 4) Spare parts & inventory (your friend's script, unchanged)
-- =========================================================

DROP TABLE IF EXISTS "SparePartReplenishmentRequest" CASCADE;
DROP TABLE IF EXISTS "SparePartForecast_TuHT" CASCADE;
DROP TABLE IF EXISTS "SparePartUsageHistory_TuHT" CASCADE;
DROP TABLE IF EXISTS "SparePart_TuHT" CASCADE;
DROP TABLE IF EXISTS "SparePartType_TuHT" CASCADE;
DROP TABLE IF EXISTS "Inventory_TuHT" CASCADE;

CREATE TABLE "Inventory_TuHT" (
    "InventoryID"         UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "CenterID"            UUID NOT NULL REFERENCES "Center"("CenterID") ON DELETE CASCADE,
    "Quantity"            INT DEFAULT 0,
    "MinimumStockLevel"   INT DEFAULT 0,
    "Status"              VARCHAR(50) DEFAULT 'ACTIVE',
    "IsActive"            BOOLEAN DEFAULT TRUE,
    "createdAt"           TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "updatedAt"           TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "SparePartType_TuHT" (
    "TypeID"          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name"            VARCHAR(256) NOT NULL,
    "Description"     VARCHAR(500),
    "Status"          VARCHAR(50) DEFAULT 'ACTIVE',
    "IsActive"        BOOLEAN DEFAULT TRUE,
    "createdAt"       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "updatedAt"       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "SparePart_TuHT" (
    "SparePartID"     UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "VehicleModelID"  INT,
    "InventoryID"     UUID REFERENCES "Inventory_TuHT"("InventoryID") ON DELETE SET NULL,
    "TypeID"          UUID REFERENCES "SparePartType_TuHT"("TypeID") ON DELETE SET NULL,
    "Name"            VARCHAR(256) NOT NULL,
    "UnitPrice"       DECIMAL(18,2),
    "Manufacture"     VARCHAR(256),
    "Status"          VARCHAR(50) DEFAULT 'ACTIVE',
    "IsActive"        BOOLEAN DEFAULT TRUE,
    "createdAt"       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "updatedAt"       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "SparePartUsageHistory_TuHT" (
    "UsageID"         UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "SparePartID"     UUID NOT NULL REFERENCES "SparePart_TuHT"("SparePartID") ON DELETE CASCADE,
    "CenterID"        UUID NOT NULL REFERENCES "Center"("CenterID") ON DELETE CASCADE,
    "QuantityUsed"    INT NOT NULL,
    "UsedDate"        TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "Status"          VARCHAR(50) DEFAULT 'ACTIVE',
    "IsActive"        BOOLEAN DEFAULT TRUE,
    "createdAt"       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "updatedAt"       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "SparePartForecast_TuHT" (
    "ForecastID"          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "SparePartID"         UUID NOT NULL REFERENCES "SparePart_TuHT"("SparePartID") ON DELETE CASCADE,
    "CenterID"            UUID NOT NULL REFERENCES "Center"("CenterID") ON DELETE CASCADE,
    "PredictedUsage"      INT DEFAULT 0,
    "SafetyStock"         INT DEFAULT 0,
    "ReorderPoint"        INT DEFAULT 0,
    "ForecastedBy"        VARCHAR(64) DEFAULT 'AI',
    "ForecastConfidence"  DECIMAL(5,2),
    "ForecastDate"        TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "ApprovedBy"          UUID,
    "ApprovedDate"        TIMESTAMP,
    "Status"              VARCHAR(50) DEFAULT 'PENDING',
    "createdAt"           TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "updatedAt"           TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "SparePartReplenishmentRequest" (
    "RequestID"       UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "CenterID"        UUID NOT NULL REFERENCES "Center"("CenterID") ON DELETE CASCADE,
    "SparePartID"     UUID NOT NULL REFERENCES "SparePart_TuHT"("SparePartID") ON DELETE CASCADE,
    "ForecastID"      UUID REFERENCES "SparePartForecast_TuHT"("ForecastID") ON DELETE SET NULL,
    "SuggestedQuantity" INT DEFAULT 0,
    "Status"          VARCHAR(50) DEFAULT 'PENDING',
    "ApprovedBy"      UUID,
    "ApprovedAt"      TIMESTAMP,
    "Notes"           VARCHAR(500),
    "createdAt"       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "updatedAt"       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TRIGGER trg_update_inventory_updatedAt
BEFORE UPDATE ON "Inventory_TuHT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

CREATE TRIGGER trg_update_spareparttype_updatedAt
BEFORE UPDATE ON "SparePartType_TuHT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

CREATE TRIGGER trg_update_sparepart_updatedAt
BEFORE UPDATE ON "SparePart_TuHT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

CREATE TRIGGER trg_update_usage_updatedAt
BEFORE UPDATE ON "SparePartUsageHistory_TuHT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

CREATE TRIGGER trg_update_forecast_updatedAt
BEFORE UPDATE ON "SparePartForecast_TuHT"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

CREATE TRIGGER trg_update_replenishment_updatedAt
BEFORE UPDATE ON "SparePartReplenishmentRequest"
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

-- =========================================================
-- INDEXES FOR TABLES WITH SUFFIX "ThaoNTT"
-- =========================================================

-- BookingScheduleThaoNTT
CREATE INDEX idx_thaontt_sched_center ON "BookingScheduleThaoNTT" ("CenterId");
CREATE INDEX idx_thaontt_sched_time   ON "BookingScheduleThaoNTT" ("StartUtc", "EndUtc");

-- BookingThaoNTT
CREATE INDEX idx_thaontt_booking_customer  ON "BookingThaoNTT" ("CustomerId");
CREATE INDEX idx_thaontt_booking_vehicle   ON "BookingThaoNTT" ("VehicleId");
CREATE INDEX idx_thaontt_booking_schedule  ON "BookingThaoNTT" ("BookingScheduleId");
CREATE INDEX idx_thaontt_booking_status    ON "BookingThaoNTT" ("Status");

-- ServiceIntakeThaoNTT
CREATE INDEX idx_thaontt_intake_booking  ON "ServiceIntakeThaoNTT" ("BookingId");
CREATE INDEX idx_thaontt_intake_advisor  ON "ServiceIntakeThaoNTT" ("AdvisorId");

-- ChecklistItemThaoNTT
CREATE INDEX idx_thaontt_chkitem_code    ON "ChecklistItemThaoNTT" ("Code");
CREATE INDEX idx_thaontt_chkitem_type    ON "ChecklistItemThaoNTT" ("Type");

-- ChecklistResponseThaoNTT
CREATE INDEX idx_thaontt_chkresp_intake  ON "ChecklistResponseThaoNTT" ("IntakeId");
CREATE INDEX idx_thaontt_chkresp_item    ON "ChecklistResponseThaoNTT" ("ItemId");
CREATE INDEX idx_thaontt_chkresp_severity ON "ChecklistResponseThaoNTT" ("Severity");

-- AssignmentThaoNTT
CREATE INDEX idx_thaontt_assign_booking  ON "AssignmentThaoNTT" ("BookingId");
CREATE INDEX idx_thaontt_assign_tech     ON "AssignmentThaoNTT" ("TechnicianId");
CREATE INDEX idx_thaontt_assign_status   ON "AssignmentThaoNTT" ("Status");

-- OrderThaoNTT
CREATE INDEX idx_thaontt_order_customer  ON "OrderThaoNTT" ("CustomerId");
CREATE INDEX idx_thaontt_order_vehicle   ON "OrderThaoNTT" ("VehicleId");
CREATE INDEX idx_thaontt_order_booking   ON "OrderThaoNTT" ("BookingId");
CREATE INDEX idx_thaontt_order_status    ON "OrderThaoNTT" ("Status");

-- OrderServiceThaoNTT
CREATE INDEX idx_thaontt_os_order        ON "OrderServiceThaoNTT" ("OrderId");
CREATE INDEX idx_thaontt_os_service      ON "OrderServiceThaoNTT" ("ServiceId");

-- WorkOrderApprovalThaoNTT
CREATE INDEX idx_thaontt_woa_order       ON "WorkOrderApprovalThaoNTT" ("OrderId");
CREATE INDEX idx_thaontt_woa_status      ON "WorkOrderApprovalThaoNTT" ("Status");
CREATE INDEX idx_thaontt_woa_method      ON "WorkOrderApprovalThaoNTT" ("Method");