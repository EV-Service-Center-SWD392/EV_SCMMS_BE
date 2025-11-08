# Sparepart Management Module - Class Diagrams

This folder contains class diagrams for the Sparepart Management Module, broken down by functional area for clarity.

## 📁 Diagram Files

### 1. **Sparepart Management** (`01_Sparepart_ClassDiagram.puml`)
**Purpose**: Core sparepart CRUD operations and catalog management

**Key Components**:
- `SparepartController` - WebAPI endpoints for sparepart operations
- `ISparepartService` - Business logic for sparepart management
- `ISparepartRepository` - Data access for spareparts
- `SparepartTuht` - Domain entity representing a sparepart

**Main Features**:
- Get all spareparts
- Get sparepart by ID
- Create/Update/Delete spareparts
- Get spareparts by type
- Get spareparts by inventory

---

### 2. **Sparepart Type Management** (`02_SparepartType_ClassDiagram.puml`)
**Purpose**: Sparepart categorization and type management

**Key Components**:
- `SparepartTypeController` - WebAPI endpoints for sparepart type operations
- `ISparepartTypeService` - Business logic for type management
- `ISparepartTypeRepository` - Data access for sparepart types
- `SpareparttypeTuht` - Domain entity for sparepart categories

**Main Features**:
- Get all sparepart types
- Create/Update/Delete types
- Categorize spareparts

---

### 3. **AI Forecast & Chatbot** (`03_SparepartForecast_AI_ClassDiagram.puml`)
**Purpose**: AI-powered demand forecasting and predictive analytics

**Key Components**:
- `SparepartForecastController` - WebAPI endpoints for forecast operations
- `ISparepartForecastService` - Business logic for AI forecasting
- `ISparepartForecastRepository` - Data access for forecasts
- `SparepartforecastTuht` - Domain entity storing forecast data

**Main Features**:
- Create AI-generated forecasts
- Get forecasts by sparepart/center
- Approve forecasts
- Track prediction confidence and safety stock levels
- **AI Integration**: Forecasted by AI chatbot with confidence metrics

---

### 4. **Replenishment Request & Approval Workflow** (`04_ReplenishmentRequest_ClassDiagram.puml`)
**Purpose**: Sparepart replenishment request management with approval/rejection workflow

**Key Components**:
- `SparepartReplenishmentRequestController` - WebAPI endpoints for replenishment requests
- `ISparepartReplenishmentRequestService` - Business logic for request workflow
- `ISparepartReplenishmentRequestRepository` - Data access for requests
- `Sparepartreplenishmentrequest` - Domain entity for replenishment requests

**Main Features**:
- Create replenishment requests (manual or AI-generated)
- **Approve requests** with approval tracking (ApproveRequestDto)
- **Reject requests** with reason and audit trail (RejectRequestDto)
- Get pending/approved/rejected requests
- Filter by status
- Link to AI forecasts

**Workflow States**: PENDING → APPROVED/REJECTED

---

### 5. **Usage History Tracking** (`05_SparepartUsageHistory_ClassDiagram.puml`)
**Purpose**: Track sparepart consumption and usage patterns

**Key Components**:
- `SparepartUsageHistoryController` - WebAPI endpoints for usage tracking
- `ISparepartUsageHistoryService` - Business logic for usage history
- `ISparepartUsageHistoryRepository` - Data access for usage records
- `SparepartusagehistoryTuht` - Domain entity for usage tracking

**Main Features**:
- Record sparepart usage
- Track quantity used and date
- Link to order spareparts
- Get usage history by sparepart/center
- **Data for AI**: Usage history feeds into AI forecasting

---

### 6. **Inventory & Stock Management** (`06_Inventory_ClassDiagram.puml`)
**Purpose**: Inventory tracking and low stock alerts

**Key Components**:
- `InventoryController` - WebAPI endpoints for inventory operations
- `IInventoryService` - Business logic for inventory management
- `IInventoryRepository` - Data access for inventory
- `InventoryTuht` - Domain entity for inventory tracking

**Main Features**:
- Get inventory by center
- Update stock quantities
- Set minimum stock levels
- **Check low stock** - Triggers replenishment alerts
- Track inventory status

---

## 🏗️ Architecture Layers

All diagrams follow a consistent **4-layer architecture**:

### 🔵 **WebAPI Layer** (LightBlue)
- Controllers handling HTTP requests
- Input validation
- Route mapping

### 🟢 **Application Layer** (LightGreen)
- Service interfaces defining business contracts
- Repository interfaces for data access
- DTOs (Data Transfer Objects)
- UnitOfWork pattern for transaction management

### 🟡 **Infrastructure Layer** (LightYellow)
- Service implementations with business logic
- Repository implementations with EF Core
- Database context integration

### 🔴 **Domain Layer** (LightCoral)
- Entity models (database tables)
- Business rules and constraints
- Entity relationships

---

## 🔄 Complete Workflow Example

**AI-Driven Replenishment Flow**:

1. **Usage Tracking** (Diagram 5)
   - Technician uses sparepart during service
   - System records usage in `SparepartusagehistoryTuht`

2. **AI Forecasting** (Diagram 3)
   - AI analyzes usage patterns
   - Generates `SparepartforecastTuht` with predicted demand
   - Sets safety stock and reorder point

3. **Inventory Check** (Diagram 6)
   - System checks current inventory levels
   - Compares with minimum stock level
   - Triggers low stock alert

4. **Replenishment Request** (Diagram 4)
   - Auto-generates `Sparepartreplenishmentrequest` based on forecast
   - Status: PENDING
   - Links to AI forecast for justification

5. **Approval Workflow** (Diagram 4)
   - Manager reviews request
   - **Approves**: Sets ApprovedBy, ApprovedAt, Status=APPROVED
   - **Rejects**: Sets RejectedBy, Reason, Status=REJECTED with audit trail

6. **Procurement** (External)
   - Approved requests forwarded to procurement
   - Updates inventory when stock arrives

---

## 📊 Key Relationships

- `SparepartTuht` → `SpareparttypeTuht`: Each sparepart has a type
- `SparepartTuht` → `InventoryTuht`: Spareparts tracked in inventory
- `SparepartforecastTuht` → `SparepartTuht`: Forecasts predict sparepart demand
- `Sparepartreplenishmentrequest` → `SparepartforecastTuht`: Requests based on forecasts
- `SparepartusagehistoryTuht` → `SparepartTuht`: Usage patterns inform forecasts
- All entities → `Centertuantm`: Multi-center support

---

## 🎨 How to View

### Using VS Code:
1. Install **PlantUML** extension
2. Open any `.puml` file
3. Press `Alt+D` (Windows/Linux) or `Option+D` (Mac) to preview

### Using Online Tools:
- [PlantUML Web Server](http://www.plantuml.com/plantuml/uml/)
- Copy/paste diagram code
- View rendered diagram

---

## 🔧 Technology Stack

- **Backend**: ASP.NET Core 8.0
- **Database**: PostgreSQL (via Supabase)
- **ORM**: Entity Framework Core
- **Pattern**: Repository + UnitOfWork
- **Architecture**: Clean Architecture (Layered)
- **AI**: Forecast confidence tracking for chatbot integration

---

## 📝 Notes

- All diagrams use **ortho linetype** for clear vertical/horizontal lines
- Color coding consistent across all diagrams
- DTOs separate from domain entities for clean separation of concerns
- Approval/Rejection workflow includes audit trail in Notes field
- AI forecast confidence tracked as decimal (0.0 - 1.0)

---

**Generated**: November 8, 2025  
**Module**: Sparepart Management with AI Forecasting and Approval Workflow
