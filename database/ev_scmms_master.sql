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
    IsActive            BOOLEAN DEFAULT TRUE,                             -- Đang hoạt động hay không
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
    IsActive        BOOLEAN DEFAULT TRUE                              -- Đang hoạt động hay không
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

DROP TABLE IF EXISTS Role CASCADE;

CREATE TABLE Role (
    RoleId      UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name        VARCHAR(128) NOT NULL,         -- tên role (ví dụ: Admin, Staff)
    Description VARCHAR(500)
);


-- Bảng AppUser (User)
DROP TABLE IF EXISTS AppUser CASCADE;

CREATE TABLE AppUser (
    UserId      UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Email       VARCHAR(256) NOT NULL UNIQUE,     -- duy nhất
    Password    VARCHAR(256) NOT NULL,            -- lưu hashed password (không lưu plaintext)
    LastName    VARCHAR(128),
    FirstName   VARCHAR(128),
    Birthday    DATE,
    Address     VARCHAR(256),
    PhoneNumber VARCHAR(32) UNIQUE,
    RoleId      UUID NOT NULL REFERENCES Role(RoleId) ON DELETE RESTRICT, -- 1-n Role -> AppUser
    Status      VARCHAR(50) DEFAULT 'ACTIVE',
    IsActive    BOOLEAN DEFAULT TRUE,
    createdAt   TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updatedAt   TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Trigger cập nhật updatedAt cho AppUser
DROP TRIGGER IF EXISTS trg_update_updatedAt_appuser ON AppUser;
CREATE TRIGGER trg_update_updatedAt_appuser
BEFORE UPDATE ON AppUser
FOR EACH ROW
EXECUTE PROCEDURE update_updatedAt();

INSERT INTO Role (Name, Description)
VALUES
    ('ADMIN', 'Quản trị viên hệ thống — có toàn quyền quản lý người dùng, trung tâm, kho và dữ liệu hệ thống.'),
    ('STAFF', 'Nhân viên điều phối hoặc hỗ trợ quản lý nghiệp vụ, có quyền xem và cập nhật dữ liệu nghiệp vụ trong phạm vi cho phép.'),
    ('TECHNICIAN', 'Kỹ thuật viên — chịu trách nhiệm bảo trì, thay thế và cập nhật lịch sử sử dụng phụ tùng tại trung tâm.'),
    ('CUSTOMER', 'Khách hàng hoặc người dùng cuối — có thể gửi yêu cầu, xem trạng thái dịch vụ và lịch sử bảo trì.');

drop table if exists refreshtoken cascade;
CREATE TABLE IF NOT EXISTS refreshtoken (
    tokenid UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    userid UUID NOT NULL,
    token VARCHAR(512) UNIQUE NOT NULL,
    expiresat TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    createdat TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT refreshtoken_userid_fkey FOREIGN KEY (userid) 
        REFERENCES appuser(userid) ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_refreshtoken_userid ON refreshtoken(userid);
CREATE INDEX IF NOT EXISTS idx_refreshtoken_token ON refreshtoken(token);
CREATE INDEX IF NOT EXISTS idx_refreshtoken_expiresat ON refreshtoken(expiresat);

-- Optional: Create a view to get active (non-expired) refresh tokens\
drop view if exists activerefreshtokens; 
CREATE OR REPLACE VIEW activerefreshtokens AS
SELECT 
    tokenid,
    userid,
    token,
    expiresat,
    createdat
FROM refreshtoken
WHERE expiresat > CURRENT_TIMESTAMP;

