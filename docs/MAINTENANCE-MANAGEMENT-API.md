# Maintenance Management API

## 1. Overview

The Maintenance Management API is part of the Logistics & Field Operations Management System and is used to manage asset maintenance activities.

Key Features:

- Maintenance Plan Management
- Maintenance Work Order Management
- Maintenance Record Management
- Maintenance Status Workflow
- Maintenance Cost Tracking

## 2. Base URL

```text
http://localhost:5152/api/maintenance
```

Adjust the port to match the API configuration currently in use.

---

## 3. Maintenance Plans

Used to manage asset maintenance schedules.

| Method | Endpoint | Description |
|---|---|---|
| GET | `/plans` | Get all maintenance plans |
| GET | `/plans/{id}` | Get maintenance plan by ID |
| POST | `/plans` | Create maintenance plan |
| PUT | `/plans/{id}` | Update maintenance plan |

### GET /plans

Query parameters:

| Parameter | Type | Required | Description |
|---|---|---|---|
| search | string | No | Search maintenance plan |
| assetId | Guid | No | Filter by asset |
| isActive | bool | No | Filter active status |

Example:

```http
GET /api/maintenance/plans?search=monthly&isActive=true
```

Response: `200 OK`

### GET /plans/{id}

Retrieve maintenance plan details based on the ID.

Response:
- `200 OK`
- `404 Not Found`

### POST /plans

Create a new maintenance plan.

Response:
- `201 Created`
- `400 Bad Request`
- `404 Not Found`

### PUT /plans/{id}

Update the maintenance plan.

Response:
- `200 OK`
- `400 Bad Request`
- `404 Not Found`

---

## 4. Maintenance Work Orders

Used to manage asset maintenance tasks.

| Method | Endpoint | Description |
|---|---|---|
| GET | `/work-orders` | Get all work orders |
| GET | `/work-orders/{id}` | Get work order by ID |
| POST | `/work-orders` | Create work order |
| PUT | `/work-orders/{id}` | Update work order |
| PATCH | `/work-orders/{id}/status` | Update work order status |

### GET /work-orders

Query parameters:

| Parameter | Type | Required | Description |
|---|---|---|---|
| search | string | No | Search work order |
| assetId | Guid | No | Filter by asset |
| status | string | No | Filter by status |
| priority | string | No | Filter by priority |
| maintenanceType | string | No | Filter by maintenance type |

Example:

```http
GET /api/maintenance/work-orders?status=OPEN&priority=HIGH
```

Response: `200 OK`

### GET /work-orders/{id}

Retrieve work order details by ID.

Response:
- `200 OK`
- `404 Not Found`

### POST /work-orders

Create a new maintenance work order.

Response:
- `201 Created`
- `400 Bad Request`
- `404 Not Found`

### PUT /work-orders/{id}

Updating maintenance work order information.

Response:
- `200 OK`
- `400 Bad Request`
- `404 Not Found`
- `409 Conflict`

### PATCH /work-orders/{id}/status

Update the status of a maintenance work order.

Supported statuses:

- `OPEN`
- `IN_PROGRESS`
- `COMPLETED`
- `CANCELLED`

The initial status of a Work Order is `OPEN`.

Response:
- `200 OK`
- `400 Bad Request`
- `404 Not Found`
- `409 Conflict`

---

## 5. Maintenance Records

Used to record maintenance activities that have been performed.

| Method | Endpoint | Description |
|---|---|---|
| GET | `/records` | Get all maintenance records |
| GET | `/records/{id}` | Get record by ID |
| POST | `/records` | Create maintenance record |

### GET /records

Query parameters:

| Parameter | Type | Required | Description |
|---|---|---|---|
| maintenanceWorkOrderId | Guid | No | Filter by work order |

Example:

```http
GET /api/maintenance/records?maintenanceWorkOrderId={workOrderId}
```

Response:
- `200 OK`

### GET /records/{id}

Retrieve maintenance record details by ID.

Response:
- `200 OK`
- `404 Not Found`

### POST /records

Create a new maintenance record.

Maintenance records can only be added to Work Orders with a status of `OPEN` or `IN_PROGRESS`.

After the record is created:
- The Work Order status changes to `IN_PROGRESS`.
- Actual Cost is updated based on the total Labor Cost and Parts Cost for the entire record.
- The Maintenance Date is saved in UTC.
  
Response:
- `201 Created`
- `400 Bad Request`
- `404 Not Found`
- `409 Conflict`
- `500 Internal Server Error`

---

## 6. Maintenance Work Order Status Workflow

Alur perubahan status:

```text
OPEN
  |
  +------> IN_PROGRESS
  |              |
  |              +------> COMPLETED
  |              |
  |              +------> CANCELLED
  |
  +------> CANCELLED
```

Rules:

- New Work Orders are created with the status `OPEN`.
- `OPEN` can change to `IN_PROGRESS` or `CANCELLED`.
- `IN_PROGRESS` can change to `COMPLETED` or `CANCELLED`.
- A Work Order can only be completed if it has at least one Maintenance Record.
- Work Orders with the status `COMPLETED` or `CANCELLED` are final statuses and cannot be changed.
- The Completion Date will be filled in when the Work Order is successfully completed.
---

## 7. Maintenance Cost Calculation

Actual Cost is calculated based on the total cost of all Maintenance Records on the Work Order.

Formula:

```text
Actual Cost = Total Labor Cost + Total Parts Cost
```

Contoh:

| Component | Amount |
|---|---:|
| Labor Cost | 200,000 |
| Parts Cost | 150,000 |
| Actual Cost | 350,000 |

“Actual Cost” is updated when a new Maintenance Record is successfully saved.

---

## 8. HTTP Status Codes

| HTTP Code | Description |
|---|---|
| 200 | Request successful |
| 201 | Resource successfully created |
| 400 | Invalid request or validation error |
| 404 | Resource not found |
| 409 | Business rule conflict |
| 500 | Unexpected server error |

---

## 9. Error Response Format

Contoh:

```json
{
  "message": "Maintenance Work Order was not found."
}
```

---

## 10. API Controller

Controller:

```text
backend/src/Logistics.API/Controllers/MaintenanceController.cs
```

Service:

```text
backend/src/Logistics.Application/Services/Maintenance/MaintenanceService.cs
```

Repository:

```text
backend/src/Logistics.Infrastructure/Persistence/Repositories/Maintenance/MaintenanceRepository.cs
```

Interface:

```text
backend/src/Logistics.Application/Interfaces/Maintenance/IMaintenanceService.cs
backend/src/Logistics.Application/Interfaces/Maintenance/IMaintenanceRepository.cs
```
