CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- =========================================================
-- Hàm trigger cập nhật tự động cột updatedAt khi có UPDATE
-- =========================================================
CREATE OR REPLACE FUNCTION update_updatedAt()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updatedAt = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


DROP TABLE IF EXISTS SparePartReplenishmentRequest CASCADE;
DROP TABLE IF EXISTS SparePartForecast_TuHT CASCADE;
DROP TABLE IF EXISTS SparePartUsageHistory_TuHT CASCADE;
DROP TABLE IF EXISTS SparePart_TuHT CASCADE;
DROP TABLE IF EXISTS SparePartType_TuHT CASCADE;
DROP TABLE IF EXISTS Inventory_TuHT CASCADE;
DROP TABLE IF EXISTS Center CASCADE;

CREATE TABLE Center (
    CenterID        UUID PRIMARY KEY DEFAULT uuid_generate_v4(),      -- ID trung tâm dịch vụ
    Name            VARCHAR(256) NOT NULL,                            -- Tên trung tâm
    Address         VARCHAR(256),                                     -- Địa chỉ trung tâm
    Status          VARCHAR(50) DEFAULT 'ACTIVE',                     -- Trạng thái hoạt động
    IsActive        BOOLEAN DEFAULT TRUE,                             -- Đang hoạt động hay không
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,              -- Ngày tạo bản ghi
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP               -- Ngày cập nhật bản ghi
);

CREATE TABLE Inventory_TuHT (
    InventoryID         UUID PRIMARY KEY DEFAULT uuid_generate_v4(),  -- ID kho phụ tùng
    CenterID            UUID NOT NULL REFERENCES Center(CenterID) ON DELETE CASCADE,  -- Liên kết trung tâm
    Quantity            INT DEFAULT 0,                                -- Số lượng tồn hiện tại
    MinimumStockLevel   INT DEFAULT 0,                                -- Ngưỡng tồn tối thiểu
    Status              VARCHAR(50) DEFAULT 'ACTIVE',                 -- Trạng thái
    IsActive            BOOLEAN DEFAULT TRUE,                         -- Còn hoạt động hay không
    createdAt           TIMESTAMP DEFAULT CURRENT_TIMESTAMP,          -- Ngày tạo
    updatedAt           TIMESTAMP DEFAULT CURRENT_TIMESTAMP           -- Ngày cập nhật
);

CREATE TABLE SparePartType_TuHT (
    TypeID          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),      -- ID loại phụ tùng
    Name            VARCHAR(256) NOT NULL,                            -- Tên loại phụ tùng
    Description     VARCHAR(500),                                     -- Mô tả loại phụ tùng
    Status          VARCHAR(50) DEFAULT 'ACTIVE',                     -- Trạng thái
    IsActive        BOOLEAN DEFAULT TRUE,                             -- Hoạt động / ngưng
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,              -- Ngày tạo
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP               -- Ngày cập nhật
);

CREATE TABLE SparePart_TuHT (
    SparePartID     UUID PRIMARY KEY DEFAULT uuid_generate_v4(),      -- ID phụ tùng
    VehicleModelID  INT,                                              -- ID dòng xe áp dụng
    InventoryID     UUID REFERENCES Inventory_TuHT(InventoryID) ON DELETE SET NULL,  -- Kho chứa phụ tùng
    TypeID          UUID REFERENCES SparePartType_TuHT(TypeID) ON DELETE SET NULL,   -- Loại phụ tùng
    Name            VARCHAR(256) NOT NULL,                            -- Tên phụ tùng
    UnitPrice       DECIMAL(18,2),                                    -- Giá đơn vị
    Manufacture     VARCHAR(256),                                     -- Nhà sản xuất
    Status          VARCHAR(50) DEFAULT 'ACTIVE',                     -- Trạng thái
    IsActive        BOOLEAN DEFAULT TRUE,                             -- Còn kinh doanh hay không
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,              -- Ngày tạo
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP               -- Ngày cập nhật
);

CREATE TABLE SparePartUsageHistory_TuHT (
    UsageID         UUID PRIMARY KEY DEFAULT uuid_generate_v4(),      -- ID lịch sử sử dụng
    SparePartID     UUID NOT NULL REFERENCES SparePart_TuHT(SparePartID) ON DELETE CASCADE,  -- Phụ tùng được dùng
    CenterID        UUID NOT NULL REFERENCES Center(CenterID) ON DELETE CASCADE,  -- Trung tâm thực hiện
    QuantityUsed    INT NOT NULL,                                     -- Số lượng sử dụng
    UsedDate        TIMESTAMP DEFAULT CURRENT_TIMESTAMP,              -- Ngày sử dụng
    Status          VARCHAR(50) DEFAULT 'ACTIVE',                     -- Trạng thái
    IsActive        BOOLEAN DEFAULT TRUE,                             -- Còn hiệu lực hay không
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,              -- Ngày tạo
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP               -- Ngày cập nhật
);

CREATE TABLE SparePartForecast_TuHT (
    ForecastID          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),  -- ID dự báo
    SparePartID         UUID NOT NULL REFERENCES SparePart_TuHT(SparePartID) ON DELETE CASCADE, -- Phụ tùng được dự báo
    CenterID            UUID NOT NULL REFERENCES Center(CenterID) ON DELETE CASCADE, -- Trung tâm dự báo
    PredictedUsage      INT DEFAULT 0,                                -- Lượng tiêu thụ dự đoán
    SafetyStock         INT DEFAULT 0,                                -- Mức tồn kho an toàn
    ReorderPoint        INT DEFAULT 0,                                -- Ngưỡng đặt hàng lại
    ForecastedBy        VARCHAR(64) DEFAULT 'AI',                     -- Người / hệ thống dự báo
    ForecastConfidence  DECIMAL(5,2),                                 -- Độ tin cậy dự báo (%)
    ForecastDate        TIMESTAMP DEFAULT CURRENT_TIMESTAMP,          -- Ngày dự báo
    ApprovedBy          UUID,                                         -- Người duyệt
    ApprovedDate        TIMESTAMP,                                    -- Ngày duyệt
    Status              VARCHAR(50) DEFAULT 'PENDING',                -- Trạng thái duyệt
    createdAt           TIMESTAMP DEFAULT CURRENT_TIMESTAMP,          -- Ngày tạo
    updatedAt           TIMESTAMP DEFAULT CURRENT_TIMESTAMP           -- Ngày cập nhật
);

CREATE TABLE SparePartReplenishmentRequest (
    RequestID       UUID PRIMARY KEY DEFAULT uuid_generate_v4(),      -- ID yêu cầu bổ sung
    CenterID        UUID NOT NULL REFERENCES Center(CenterID) ON DELETE CASCADE,  -- Trung tâm gửi yêu cầu
    SparePartID     UUID NOT NULL REFERENCES SparePart_TuHT(SparePartID) ON DELETE CASCADE, -- Phụ tùng cần bổ sung
    ForecastID      UUID REFERENCES SparePartForecast_TuHT(ForecastID) ON DELETE SET NULL,  -- Tham chiếu dự báo
    SuggestedQuantity INT DEFAULT 0,                                  -- Số lượng đề xuất nhập
    Status          VARCHAR(50) DEFAULT 'PENDING',                    -- Trạng thái yêu cầu
    ApprovedBy      UUID,                                             -- Người duyệt
    ApprovedAt      TIMESTAMP,                                        -- Ngày duyệt
    Notes           VARCHAR(500),                                     -- Ghi chú thêm
    createdAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,              -- Ngày tạo
    updatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP               -- Ngày cập nhật
);

CREATE TRIGGER trg_update_center_updatedAt
BEFORE UPDATE ON Center
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

CREATE TRIGGER trg_update_inventory_updatedAt
BEFORE UPDATE ON Inventory_TuHT
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

CREATE TRIGGER trg_update_spareparttype_updatedAt
BEFORE UPDATE ON SparePartType_TuHT
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

CREATE TRIGGER trg_update_sparepart_updatedAt
BEFORE UPDATE ON SparePart_TuHT
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

CREATE TRIGGER trg_update_usage_updatedAt
BEFORE UPDATE ON SparePartUsageHistory_TuHT
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

CREATE TRIGGER trg_update_forecast_updatedAt
BEFORE UPDATE ON SparePartForecast_TuHT
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();

CREATE TRIGGER trg_update_replenishment_updatedAt
BEFORE UPDATE ON SparePartReplenishmentRequest
FOR EACH ROW EXECUTE FUNCTION update_updatedAt();
