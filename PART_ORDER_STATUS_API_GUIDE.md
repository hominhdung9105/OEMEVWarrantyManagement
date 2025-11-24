# Part Order Status API Guide

## Overview
The Part Order API endpoints now accept status as a **string** parameter using the **description** from the `PartOrderStatus` enum, not the enum name directly.

## Changes Made

### 1. Added Helper Method
Added `FromDescription()` method to `PartOrderStatusExtensions` to parse status from description string:

```csharp
public static PartOrderStatus? FromDescription(string? description)
```

### 2. Updated Controller Endpoints

#### GET /api/PartOrder
- **Parameter**: `status` (string, optional)
- **Before**: Accepted enum directly (e.g., `PartOrderStatus.Pending`)
- **After**: Accepts description string (e.g., `"Pending"`, `"In Transit"`)

#### GET /api/PartOrder/count
- **Parameter**: `status` (string, default: `"Pending"`)
- **Before**: Accepted enum directly
- **After**: Accepts description string

### 3. Added Error Handling
- New error: `ResponseError.InvalidStatus` (code: 78)
- Returns custom message with invalid status value

## Valid Status Values

Use these **exact strings** (case-insensitive) in API requests:

| Description String | Enum Value | Use Case |
|-------------------|------------|----------|
| `"Pending"` | `Pending` | Order created, waiting for approval |
| `"Waiting"` | `Waiting` | Waiting for expected date |
| `"Confirmed"` | `Confirm` | Order confirmed by EVM |
| `"In Transit"` | `InTransit` | Order shipped to SC |
| `"Delivered"` | `Delivery` | Order delivered to SC |
| `"Done"` | `Done` | Order completed |
| `"Cancelled"` | `Cancelled` | Order cancelled (lost) |
| `"Returning"` | `Returning` | Order being returned to EVM |
| `"Return Inspection"` | `ReturnInspection` | Return being inspected |
| `"Discrepancy Review"` | `DiscrepancyReview` | Discrepancy under review |

## API Examples

### Example 1: Get all orders with specific status
```http
GET /api/PartOrder?page=0&size=10&status=In Transit
```

### Example 2: Get all pending orders
```http
GET /api/PartOrder?page=0&size=10&status=Pending
```

### Example 3: Count orders by status
```http
GET /api/PartOrder/count?status=Confirmed&orgId=123e4567-e89b-12d3-a456-426614174000
```

### Example 4: Count pending orders (default)
```http
GET /api/PartOrder/count
```

## Error Response

If an invalid status is provided:

```json
{
  "success": false,
  "code": 78,
  "message": "Invalid status: InvalidStatusName",
  "data": null
}
```

## Implementation Details

### Controller Logic
```csharp
PartOrderStatus? partOrderStatus = null;
if (!string.IsNullOrWhiteSpace(status))
{
    partOrderStatus = PartOrderStatusExtensions.FromDescription(status);
    if (partOrderStatus == null)
    {
        return BadRequest(ApiResponse<object>.Fail(
            ResponseError.InvalidStatus, 
            $"Invalid status: {status}"));
    }
}
```

### Extension Method
```csharp
public static PartOrderStatus? FromDescription(string? description)
{
    if (string.IsNullOrWhiteSpace(description))
        return null;

    foreach (PartOrderStatus status in Enum.GetValues(typeof(PartOrderStatus)))
    {
        var memberInfo = typeof(PartOrderStatus).GetField(status.ToString());
        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(
            memberInfo, typeof(DescriptionAttribute));
        
        if (attribute?.Description != null && 
            string.Equals(attribute.Description, description, 
                StringComparison.OrdinalIgnoreCase))
        {
            return status;
        }
    }

    return null;
}
```

## Migration Notes

**Breaking Changes:**
- Client code that was passing enum names directly must update to use description strings
- For example: Change `status=Pending` to `status=Pending` (in this case same, but `status=Confirm` ? `status=Confirmed`)

**Notable Changes:**
- `Confirm` ? `"Confirmed"`
- `InTransit` ? `"In Transit"`
- `Delivery` ? `"Delivered"`
- `ReturnInspection` ? `"Return Inspection"`
- `DiscrepancyReview` ? `"Discrepancy Review"`
