# Assignment Cancel API - Frontend Integration Guide

## 📌 Tổng quan

API `DELETE /api/assignment/{id}` đã được cập nhật để trả về thông tin chi tiết hơn sau khi cancel assignment, giúp frontend biết được booking có thể reassign ngay hay không.

---

## 🔄 Thay đổi API

### ❌ **Cũ (Deprecated)**
```typescript
// Response cũ - chỉ trả về success message
DELETE /api/assignment/{id}

Response: {
  "success": true,
  "message": "Assignment cancelled successfully"
}
```

### ✅ **Mới (Current)**
```typescript
// Response mới - trả về thông tin đầy đủ
DELETE /api/assignment/{id}

Response: {
  "isSuccess": true,
  "data": {
    "assignmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "bookingId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "hasActiveAssignments": false,
    "bookingStatus": "APPROVED",
    "message": "Assignment cancelled successfully. Booking is now available for reassignment."
  },
  "message": "Assignment cancelled successfully. Booking is now available for reassignment."
}
```

---

## 📊 Response Schema

### Success Response (200 OK)

```typescript
interface CancelAssignmentResponse {
  isSuccess: boolean;
  data: {
    assignmentId: string;        // ID của assignment đã cancel
    bookingId: string;            // ID của booking liên quan
    hasActiveAssignments: boolean; // true = booking còn assignments khác
                                   // false = booking sẵn sàng reassign
    bookingStatus: string;        // Trạng thái hiện tại của booking
    message: string;              // Thông báo chi tiết
  };
  message: string;
}
```

### Error Response (400 Bad Request)

```typescript
{
  "isSuccess": false,
  "data": null,
  "message": "Assignment not found"
}
```

---

## 💡 Logic xử lý

### Case 1: Cancel assignment duy nhất
```json
// Trước khi cancel
{
  "booking": "APPROVED",
  "assignments": [
    { "id": "abc-123", "status": "ASSIGNED" }
  ]
}

// Sau khi cancel assignment abc-123
{
  "isSuccess": true,
  "data": {
    "assignmentId": "abc-123",
    "bookingId": "booking-456",
    "hasActiveAssignments": false,  // ← Không còn assignment nào
    "bookingStatus": "APPROVED",
    "message": "Assignment cancelled successfully. Booking is now available for reassignment."
  }
}

// ✅ Booking có thể reassign ngay!
```

### Case 2: Cancel một trong nhiều assignments
```json
// Trước khi cancel
{
  "booking": "APPROVED",
  "assignments": [
    { "id": "abc-123", "status": "ASSIGNED" },
    { "id": "def-456", "status": "ASSIGNED" }
  ]
}

// Sau khi cancel assignment abc-123
{
  "isSuccess": true,
  "data": {
    "assignmentId": "abc-123",
    "bookingId": "booking-789",
    "hasActiveAssignments": true,   // ← Còn assignment def-456
    "bookingStatus": "APPROVED",
    "message": "Assignment cancelled successfully"
  }
}

// ⚠️ Booking vẫn có assignment khác, chưa thể reassign
```

---

## 🔨 Frontend Implementation

### 1. TypeScript Interface

```typescript
// types/assignment.ts
export interface CancelAssignmentResponseDto {
  assignmentId: string;
  bookingId: string;
  hasActiveAssignments: boolean;
  bookingStatus: string;
  message: string;
}

export interface ApiResponse<T> {
  isSuccess: boolean;
  data: T | null;
  message: string;
}
```

### 2. API Service

```typescript
// services/assignmentService.ts
import axios from 'axios';

export class AssignmentService {
  private baseUrl = '/api/assignment';

  /**
   * Cancel một assignment
   * @param assignmentId - ID của assignment cần cancel
   * @returns Response với thông tin booking availability
   */
  async cancelAssignment(
    assignmentId: string
  ): Promise<ApiResponse<CancelAssignmentResponseDto>> {
    try {
      const response = await axios.delete<ApiResponse<CancelAssignmentResponseDto>>(
        `${this.baseUrl}/${assignmentId}`
      );
      return response.data;
    } catch (error) {
      throw error;
      }
  }

  /**
   * Tạo assignment mới cho booking
   */
  async createAssignment(data: {
    bookingId: string;
    technicianId: string;
    plannedStartUtc: string;
    plannedEndUtc: string;
  }): Promise<ApiResponse<AssignmentDto>> {
    const response = await axios.post<ApiResponse<AssignmentDto>>(
      this.baseUrl,
      data
    );
    return response.data;
  }
}
```

### 3. React Component Example

```typescript
// components/AssignmentCard.tsx
import React, { useState } from 'react';
import { AssignmentService } from '../services/assignmentService';
import { toast } from 'react-toastify';

interface AssignmentCardProps {
  assignmentId: string;
  bookingId: string;
  technicianName: string;
  onCancelled?: (bookingId: string, canReassign: boolean) => void;
}

export const AssignmentCard: React.FC<AssignmentCardProps> = ({
  assignmentId,
  bookingId,
  technicianName,
  onCancelled
}) => {
  const [loading, setLoading] = useState(false);
  const assignmentService = new AssignmentService();

  const handleCancel = async () => {
    if (!confirm(`Bạn có chắc muốn hủy assignment này?`)) return;

    setLoading(true);
    try {
      const result = await assignmentService.cancelAssignment(assignmentId);

      if (result.isSuccess && result.data) {
        // Hiển thị thông báo thành công
        toast.success(result.data.message);

        // Kiểm tra xem có thể reassign không
        if (!result.data.hasActiveAssignments) {
          toast.info('Booking này đã sẵn sàng để assign lại!');
          
          // Có thể hiện button reassign ngay
          onCancelled?.(result.data.bookingId, true);
        } else {
          toast.warning('Booking này vẫn còn assignments khác');
          onCancelled?.(result.data.bookingId, false);
        }
      }
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Có lỗi xảy ra');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="assignment-card">
      <h3>Technician: {technicianName}</h3>
      <button 
        onClick={handleCancel} 
        disabled={loading}
        className="btn-cancel"
      >
        {loading ? 'Đang xử lý...' : 'Hủy Assignment'}
      </button>
    </div>
  );
};
```

### 4. Advanced: Cancel & Reassign Flow

```typescript
// components/ReassignmentModal.tsx
import React, { useState } from 'react';

export const ReassignmentFlow = ({ assignmentId, bookingId }: Props) => {
  const [showReassignForm, setShowReassignForm] = useState(false);
  const assignmentService = new AssignmentService();

  const handleCancelAndReassign = async () => {
    try {
      // Bước 1: Cancel assignment hiện tại
      const cancelResult = await assignmentService.cancelAssignment(assignmentId);

      if (!cancelResult.isSuccess || !cancelResult.data) {
        throw new Error('Cancel failed');
      }

      // Bước 2: Kiểm tra có thể reassign không
      if (cancelResult.data.hasActiveAssignments) {
        toast.warning('Không thể reassign vì booking còn assignments khác');
        return;
      }

      // Bước 3: Hiển thị form để chọn technician mới
      setShowReassignForm(true);
      toast.success('Đã hủy assignment. Vui lòng chọn technician mới');

    } catch (error) {
      toast.error('Lỗi khi cancel assignment');
    }
  };

  const handleConfirmReassign = async (newTechnicianId: string) => {
    try {
      // Tạo assignment mới
      const result = await assignmentService.createAssignment({
        bookingId: bookingId,
        technicianId: newTechnicianId,
        plannedStartUtc: '2025-11-09T08:00:00Z',
        plannedEndUtc: '2025-11-09T10:00:00Z'
      });

      if (result.isSuccess) {
        toast.success('Đã reassign thành công!');
        setShowReassignForm(false);
      }
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Không thể reassign');
    }
  };

  return (
    <div>
      <button onClick={handleCancelAndReassign}>
        Cancel & Reassign
      </button>

      {showReassignForm && (
        <TechnicianSelectionForm 
          onConfirm={handleConfirmReassign}
          onCancel={() => setShowReassignForm(false)}
        />
      )}
    </div>
  );
};
```

---

## 🎯 Use Cases

### Use Case 1: Simple Cancel
```typescript
// Chỉ cancel, không quan tâm reassign
async function cancelAssignment(id: string) {
  const result = await assignmentService.cancelAssignment(id);
  
  if (result.isSuccess) {
    console.log('Cancelled:', result.data?.message);
    // Refresh danh sách assignments
    refreshAssignmentList();
  }
}
```

### Use Case 2: Cancel với kiểm tra availability
```typescript
// Cancel và hiển thị badge nếu booking sẵn sàng reassign
async function cancelAndCheckAvailability(id: string) {
  const result = await assignmentService.cancelAssignment(id);
  
  if (result.isSuccess && result.data) {
    if (!result.data.hasActiveAssignments) {
      // Hiển thị badge "Available for reassignment"
      showReassignBadge(result.data.bookingId);
    }
  }
}
```

### Use Case 3: Cancel & Auto Reassign
```typescript
// Cancel và tự động mở modal reassign nếu có thể
async function cancelWithAutoReassign(id: string) {
  const result = await assignmentService.cancelAssignment(id);
  
  if (result.isSuccess && result.data) {
    if (!result.data.hasActiveAssignments) {
      // Tự động mở modal reassign
      openReassignModal({
        bookingId: result.data.bookingId,
        message: 'Booking sẵn sàng. Chọn technician mới?'
      });
    } else {
      toast.info('Booking vẫn có assignments khác');
    }
  }
}
```

---

## ⚠️ Lưu ý quan trọng

### 1. Không cần API "unassign" riêng
```typescript
// ❌ KHÔNG CẦN làm thế này
await unassignBooking(bookingId);  // API này không tồn tại
await createAssignment(data);

// ✅ CHỈ CẦN làm thế này
const cancelResult = await cancelAssignment(assignmentId);
if (!cancelResult.data?.hasActiveAssignments) {
  await createAssignment(data);  // Tạo assignment mới trực tiếp
}
```

### 2. Assignment bị cancel KHÔNG bị xóa
```typescript
// Assignment vẫn còn trong database với:
// - status: "CANCELLED"
// - isActive: false
// Có thể query lại để xem lịch sử
```

### 3. Booking status không thay đổi
```typescript
// Booking vẫn giữ status "APPROVED"
// Chỉ update timestamp để tracking
// Vẫn có thể tạo assignment mới ngay lập tức
```

---

## 📋 Testing Checklist

- [ ] Test cancel assignment duy nhất → `hasActiveAssignments = false`
- [ ] Test cancel một trong nhiều assignments → `hasActiveAssignments = true`
- [ ] Test cancel assignment không tồn tại → Error 400
- [ ] Test reassign ngay sau khi cancel → Success
- [ ] Test UI hiển thị đúng message từ response
- [ ] Test UI hiện/ẩn button reassign dựa trên `hasActiveAssignments`

---

## 🔗 Related APIs

```typescript
// 1. Get assignment by ID
GET /api/assignment/{id}

// 2. Create new assignment
POST /api/assignment
Body: {
  "bookingId": "string",
  "technicianId": "string",
  "plannedStartUtc": "2025-11-09T08:00:00Z",
  "plannedEndUtc": "2025-11-09T10:00:00Z"
}

// 3. Reschedule assignment
PUT /api/assignment/{id}/reschedule
Body: {
  "plannedStartUtc": "2025-11-09T09:00:00Z",
  "plannedEndUtc": "2025-11-09T11:00:00Z"
}

// 4. Reassign to different technician
PUT /api/assignment/{id}/reassign
Body: {
  "newTechnicianId": "string"
}
```

---

## 📞 Support

Nếu có thắc mắc về API này, liên hệ Backend team hoặc tham khảo:
- Swagger UI: `/swagger`
- Source code: `EV_SCMMS.WebAPI/Controllers/AssignmentController.cs`
- Service logic: `EV_SCMMS.Infrastructure/Services/AssignmentService.cs`

---

**Last Updated**: November 8, 2025  
**API Version**: 2.0  
**Breaking Change**: No (backward compatible với response cũ thông qua `message` field)
